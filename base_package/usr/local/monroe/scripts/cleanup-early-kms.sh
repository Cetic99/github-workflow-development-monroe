#!/bin/bash
set -e

echo "Restoring initramfs modules configuration..."

MODULES_FILE="/etc/initramfs-tools/modules"
BACKUP_FILE="/etc/initramfs-tools/modules.monroe-backup"

if [ -f "$BACKUP_FILE" ]; then
    echo "Restoring original modules file from backup"
    mv "$BACKUP_FILE" "$MODULES_FILE"
    
    # Update initramfs
    echo "Updating initramfs..."
    update-initramfs -u
    
    echo "Early KMS configuration removed successfully!"
else
    echo "No backup found, skipping restore"
fi
