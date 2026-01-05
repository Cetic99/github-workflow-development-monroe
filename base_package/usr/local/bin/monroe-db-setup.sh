#!/bin/bash
# Monroe Database Setup and Migration Script
# Handles database creation and Flyway migrations

set -e

# Load environment variables from .env if it exists
ENV_FILE="/etc/monroe/.env"
if [ -f "$ENV_FILE" ]; then
    echo "Loading environment variables from $ENV_FILE..."
    set -a
    source "$ENV_FILE"
    set +a
    echo "✓ Environment variables loaded"
fi

# Configuration
DB_PATH="/usr/local/monroe/monroe-db.fdb"
DB_USER="SYSDBA"
DB_PASSWORD="${SYSDBA_FIREBIRD_PASSWORD:-masterkey}"
FLYWAY_HOME="/opt/flyway"
MIGRATIONS_DIR="/opt/monroe/migrations"
FIREBIRD_BIN="/opt/firebird/bin"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Firebird is running
check_firebird() {
    log_info "Checking Firebird status..."
    
    if ! systemctl is-active --quiet firebird; then
        log_warn "Firebird is not running. Starting Firebird..."
        systemctl start firebird
        sleep 5
    else
        log_info "Firebird is running"
    fi
}

# Setup required directories
setup_directories() {
    log_info "Setting up directories..."
    mkdir -p "$(dirname "$DB_PATH")"
    mkdir -p "$MIGRATIONS_DIR"
    mkdir -p "$FLYWAY_HOME/conf"
}

# Setup database (copy from /tmp if first install, or keep existing)
setup_database() {
    if [ ! -f "$DB_PATH" ]; then
        log_info "Database not found at $DB_PATH - this appears to be a fresh installation"
        
        # Check if template database exists in /tmp
        if [ -f "/tmp/monroe-db.fdb" ]; then
            log_info "Copying initial database from /tmp..."
            cp /tmp/monroe-db.fdb "$DB_PATH"
            
            if [ -f "$DB_PATH" ]; then
                log_info "✓ Database copied successfully"
                chown firebird:firebird "$DB_PATH"
                chmod 660 "$DB_PATH"
            else
                log_error "Failed to copy database"
                return 1
            fi
        else
            log_error "Template database not found at /tmp/monroe-db.fdb"
            log_error "Package installation may be incomplete"
            return 1
        fi
    else
        log_info "Database exists at $DB_PATH - preserving existing database"
        log_info "Migrations will be applied to update schema"
        # Ensure correct permissions
        chown firebird:firebird "$DB_PATH"
        chmod 660 "$DB_PATH"
    fi
}

# Setup Flyway configuration
setup_flyway_config() {
    log_info "Setting up Flyway configuration..."
    
    cat > "$FLYWAY_HOME/conf/flyway.conf" <<EOF
# Flyway Configuration for Monroe CashVault
flyway.url=jdbc:firebirdsql://localhost:3050/$DB_PATH?encoding=UTF8
flyway.user=$DB_USER
flyway.password=$DB_PASSWORD
flyway.locations=filesystem:$MIGRATIONS_DIR
flyway.baselineOnMigrate=true
flyway.validateOnMigrate=true
flyway.outOfOrder=false
flyway.encoding=UTF-8
flyway.table=flyway_schema_history
flyway.cleanDisabled=true
EOF

    chmod 600 "$FLYWAY_HOME/conf/flyway.conf"
    log_info "Flyway configuration created"
}

# Run database migrations
run_migrations() {
    log_info "Preparing to run database migrations..."
    
    if [ ! -d "$MIGRATIONS_DIR" ]; then
        log_warn "Migrations directory does not exist: $MIGRATIONS_DIR"
        return 0
    fi
    
    MIGRATION_COUNT=$(find "$MIGRATIONS_DIR" -name "V*.sql" 2>/dev/null | wc -l)
    
    if [ "$MIGRATION_COUNT" -eq 0 ]; then
        log_warn "No migration files found in $MIGRATIONS_DIR"
        return 0
    fi
    
    log_info "Found $MIGRATION_COUNT migration file(s)"
    
    cd "$FLYWAY_HOME"
    
    # Display current migration status
    log_info "Current migration status:"
    ./flyway info || log_warn "Could not retrieve migration info"
    
    # Execute migrations
    log_info "Executing migrations..."
    if ./flyway migrate; then
        log_info "✓ Migrations completed successfully"
        echo ""
        log_info "Updated migration status:"
        ./flyway info
    else
        log_error "✗ Migration failed!"
        return 1
    fi
}

# Note: Monroe application uses SYSDBA user directly
# No separate Monroe database user is created

# Main function
main() {
    echo ""
    log_info "=========================================="
    log_info "Monroe Database Setup & Migration"
    log_info "=========================================="
    echo ""
    
    check_firebird
    setup_directories
    setup_database
    setup_flyway_config
    run_migrations
    
    echo ""
    log_info "=========================================="
    log_info "Database setup completed successfully!"
    log_info "=========================================="
    echo ""
}

# Run main function
main "$@"
