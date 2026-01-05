#!/bin/bash
set -e

echo "Configuring early KMS for smooth Plymouth experience..."

MODULES_FILE="/etc/initramfs-tools/modules"
BACKUP_FILE="/etc/initramfs-tools/modules.monroe-backup"

# Detect GPU driver
detect_gpu_driver() {
    if lspci | grep -i vga | grep -qi intel; then
        echo "i915"
    elif lspci | grep -i vga | grep -qi amd; then
        echo "amdgpu"
    elif lspci | grep -i vga | grep -qi nvidia; then
        echo "nvidia"
    elif lspci | grep -i vga | grep -qi radeon; then
        echo "radeon"
    else
        echo ""
    fi
}

# Backup original modules file
if [ ! -f "$BACKUP_FILE" ]; then
    cp "$MODULES_FILE" "$BACKUP_FILE"
    echo "Backed up $MODULES_FILE to $BACKUP_FILE"
fi

# Detect driver
DRIVER=$(detect_gpu_driver)

if [ -z "$DRIVER" ]; then
    echo "Warning: Could not detect GPU driver automatically"
    echo "You may need to manually add your driver to $MODULES_FILE"
    exit 0
fi

echo "Detected GPU driver: $DRIVER"

# Check if driver already exists
if grep -q "^$DRIVER\$" "$MODULES_FILE"; then
    echo "$DRIVER already present in $MODULES_FILE"
else
    echo "Adding $DRIVER to $MODULES_FILE"
    echo "$DRIVER" >> "$MODULES_FILE"
fi

# Update initramfs
echo "Updating initramfs... This may take a moment."
update-initramfs -u

echo "Early KMS configured successfully!"
echo "Driver $DRIVER will load early during boot for smooth Plymouth experience."
