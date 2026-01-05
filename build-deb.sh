#!/bin/bash
set -e

BASE_DIR=$PWD

OUTDIR="$BASE_DIR/dist"
PKGDIR="$OUTDIR/package"
PKGNAME=$(grep '^Package:' $BASE_DIR/base_package/DEBIAN/control | awk '{print $2}')
VERSION=$(grep '^Version:' $BASE_DIR/base_package/DEBIAN/control | awk '{print $2}')
ARCH=$(grep '^Architecture:' $BASE_DIR/base_package/DEBIAN/control | awk '{print $2}')


mkdir -p dist/cash_vault
mkdir -p dist/cash_vault_gui
mkdir -p dist/package
rm -rf dist/package/*
rm -rf dist/cash_vault/*
rm -rf dist/cash_vault_gui/*
cp -r $BASE_DIR/base_package/* $BASE_DIR/dist/package/

# Copy database migrations to package
echo "Copying database migrations..."
mkdir -p $BASE_DIR/dist/package/tmp/migrations
cp $BASE_DIR/src/CashVault/CashVault.Infrastructure/PersistentStorage/DatabaseMigrations/V*.sql \
    $BASE_DIR/dist/package/tmp/migrations/
echo "Migrations copied: $(ls $BASE_DIR/dist/package/tmp/migrations/ | wc -l) files"

# Create empty database using local Firebird (no Docker needed)
echo "Creating initial database file..."
DB_PATH="$BASE_DIR/dist/package/tmp/monroe-db.fdb"
mkdir -p "$(dirname "$DB_PATH")"

# Extract Firebird to temporary location
TEMP_FB_DIR="/tmp/firebird-build-$$"
echo "Extracting Firebird to $TEMP_FB_DIR..."
mkdir -p "$TEMP_FB_DIR"
tar -xzf "$BASE_DIR/base_package/tmp/Firebird-5.0.3.1683-0-linux-x64/buildroot.tar.gz" -C "$TEMP_FB_DIR"

# Find Firebird binaries
FB_BIN="$TEMP_FB_DIR/opt/firebird/bin"
FB_LIB="$TEMP_FB_DIR/opt/firebird/lib"

# Set library path
export LD_LIBRARY_PATH="$FB_LIB:$LD_LIBRARY_PATH"

# Create database directory
DB_DIR="$TEMP_FB_DIR/databases"
mkdir -p "$DB_DIR"

# Start Firebird server in background
echo "Starting temporary Firebird server..."
export FIREBIRD="$TEMP_FB_DIR/opt/firebird"
export FIREBIRD_LOCK="$TEMP_FB_DIR/firebird/lock"
export FIREBIRD_MSG="$TEMP_FB_DIR/opt/firebird"
mkdir -p "$TEMP_FB_DIR/firebird/lock"

# Run firebird with minimal config
"$FB_BIN/firebird" &
FB_PID=$!
sleep 8

# Create database
echo "Creating database..."
"$FB_BIN/isql" -user SYSDBA -password masterkey <<EOF
CREATE DATABASE '$DB_DIR/monroe-db.fdb' 
    USER 'SYSDBA' 
    PASSWORD 'masterkey' 
    PAGE_SIZE 16384 
    DEFAULT CHARACTER SET NONE;
QUIT;
EOF

# Give it a moment to write
sleep 2

# Stop Firebird gracefully
kill -TERM $FB_PID 2>/dev/null
sleep 2
kill -KILL $FB_PID 2>/dev/null || true
wait $FB_PID 2>/dev/null || true

# Copy database to final location
if [ -f "$DB_DIR/monroe-db.fdb" ]; then
    cp "$DB_DIR/monroe-db.fdb" "$DB_PATH"
    echo "✓ Database created successfully ($(du -h "$DB_PATH" | cut -f1))"
    chmod 660 "$DB_PATH"
else
    echo "✗ ERROR: Failed to create database"
    echo "Checking for any .fdb files created:"
    find "$TEMP_FB_DIR" -name "*.fdb" -ls
fi

# Cleanup
rm -rf "$TEMP_FB_DIR"

if [ ! -f "$DB_PATH" ]; then
    exit 1
fi

cd src/CashVault/
dotnet-deb -c Release -o built-deb-directory/ CashVault.WebAPI/CashVault.WebAPI.csproj install
echo $PWD
cd CashVault.WebAPI
dotnet deb -o $BASE_DIR/dist/cash_vault

cd $BASE_DIR

cd $BASE_DIR/src/CashVault.GUI && npm install && npm run build:linux
cp $BASE_DIR/src/CashVault.GUI/dist/monroe-frontend_0.1.0_amd64.deb $BASE_DIR/dist/cash_vault_gui/

dpkg-deb -x $BASE_DIR/dist/cash_vault_gui/monroe-frontend_0.1.0_amd64.deb $BASE_DIR/dist/package/
dpkg-deb -x $BASE_DIR/dist/cash_vault/CashVault.WebAPI.1.0.0.linux-x64.deb  $BASE_DIR/dist/package/

# Set executable permissions for scripts
chmod +x $BASE_DIR/dist/package/DEBIAN/*
chmod 755 $BASE_DIR/dist/package/DEBIAN/*
chmod +x $BASE_DIR/dist/package/usr/local/bin/monroe-db-setup.sh

echo "Package preparation completed"

# dpkg-deb --build $BASE_DIR/dist/package
# dpkg-deb --build "$PKGDIR" "$OUTDIR/monroe-device-client_${VERSION}_${ARCH}.deb"

dpkg-deb --build "$PKGDIR" "$OUTDIR/${PKGNAME}_${VERSION}_${ARCH}.deb"

#note::::
# sudo useradd --system --no-create-home --shell /usr/sbin/nologin monroe
# sudo groupadd monroe
# sudo usermod -aG monroe monroe

# sudo chown root:root /opt/cashvault-frontend/chrome-sandbox
# sudo chmod 4755 /opt/cashvault-frontend/chrome-sandbox
