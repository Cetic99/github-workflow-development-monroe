#!/bin/bash
# Install all dependencies required for building monroe-device-client package

set -e

echo "=========================================="
echo "Installing Build Dependencies"
echo "=========================================="
echo ""

# Update package lists
echo "Updating package lists..."
sudo apt-get update

# Install .NET SDK (required for dotnet build and dotnet-deb)
echo "Installing .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 8.0
    export PATH="$HOME/.dotnet:$PATH"
    echo 'export PATH="$HOME/.dotnet:$PATH"' >> ~/.bashrc
else
    echo "✓ .NET SDK already installed"
fi

# Install Node.js and npm (required for CashVault.GUI build)
echo "Installing Node.js and npm..."
if ! command -v node &> /dev/null; then
    curl -fsSL https://deb.nodesource.com/setup_20.x | sudo -E bash -
    sudo apt-get install -y nodejs
else
    echo "✓ Node.js already installed ($(node --version))"
fi

# Install Docker (optional - no longer required for Firebird database creation)
echo "Docker is optional (Firebird is extracted from project files)..."
if ! command -v docker &> /dev/null; then
    echo "ℹ Docker not installed (not required for build)"
else
    echo "✓ Docker already installed ($(docker --version))"
fi

# Install dpkg-deb (usually pre-installed, but verify)
echo "Verifying dpkg-deb..."
if ! command -v dpkg-deb &> /dev/null; then
    sudo apt-get install -y dpkg-dev
else
    echo "✓ dpkg-deb already installed"
fi

# Install dotnet-deb tool
echo "Installing dotnet-deb tool..."
if ! dotnet tool list --global | grep -q dotnet-deb; then
    dotnet tool install --global dotnet-deb
    export PATH="$HOME/.dotnet/tools:$PATH"
    echo 'export PATH="$HOME/.dotnet/tools:$PATH"' >> ~/.bashrc
else
    echo "✓ dotnet-deb already installed"
fi

echo ""
echo "=========================================="
echo "Build Dependencies Installation Complete!"
echo "=========================================="
echo ""
echo "You can now run: ./build-deb.sh"
