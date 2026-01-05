#!/bin/bash
set -e

THEME_NAME="monroe"
THEME_DIR="/usr/share/plymouth/themes/${THEME_NAME}"
PLYMOUTH_FILE="${THEME_DIR}/${THEME_NAME}.plymouth"
BACKUP_FILE="/usr/local/monroe/.plymouth-previous-theme"

setup_plymouth_theme() {
    echo "Setting up Monroe Plymouth theme..."
    
    # Check if the theme exists
    if [ ! -f "${PLYMOUTH_FILE}" ]; then
        echo "Error: Plymouth theme file not found at ${PLYMOUTH_FILE}"
        exit 1
    fi
    
    # Save the current theme before changing (for easier restore)
    CURRENT_THEME=$(update-alternatives --query default.plymouth 2>/dev/null | grep "^Value:" | cut -d' ' -f2 || echo "")
    if [ -n "${CURRENT_THEME}" ] && [ "${CURRENT_THEME}" != "${PLYMOUTH_FILE}" ]; then
        echo "${CURRENT_THEME}" > "${BACKUP_FILE}"
        echo "Previous theme saved: ${CURRENT_THEME}"
    fi
    
    # Register the theme as an alternative (non-interactive)
    update-alternatives --install \
        /usr/share/plymouth/themes/default.plymouth \
        default.plymouth \
        "${PLYMOUTH_FILE}" \
        100
    
    # Automatically set as default (without --config that requires user input)
    update-alternatives --set default.plymouth "${PLYMOUTH_FILE}"
    
    # Update initramfs
    echo "Updating initramfs... This may take a moment."
    update-initramfs -u
    
    echo "Monroe Plymouth theme installed successfully!"
    echo "The splash screen will be visible on next boot."
}

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo "This script must be run as root"
    exit 1
fi

setup_plymouth_theme
