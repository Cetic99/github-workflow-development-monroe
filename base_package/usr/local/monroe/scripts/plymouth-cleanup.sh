#!/bin/bash
set -e

THEME_NAME="monroe"
PLYMOUTH_FILE="/usr/share/plymouth/themes/${THEME_NAME}/${THEME_NAME}.plymouth"
BACKUP_FILE="/usr/local/monroe/.plymouth-previous-theme"

cleanup_plymouth_theme() {
    echo "Removing Monroe Plymouth theme..."
    
    # Try restoring the previous theme from the backup file FIRST
    if [ -f "${BACKUP_FILE}" ]; then
        PREVIOUS_THEME=$(cat "${BACKUP_FILE}")
        if [ -f "${PREVIOUS_THEME}" ]; then
            echo "Restoring previous Plymouth theme: ${PREVIOUS_THEME}..."
            update-alternatives --set default.plymouth "${PREVIOUS_THEME}" 2>/dev/null || true
            rm -f "${BACKUP_FILE}"
        else
            echo "Previous theme file not found, will set default..."
            rm -f "${BACKUP_FILE}"
        fi
    fi
    
    # Remove the Monroe alternative
    if update-alternatives --list default.plymouth 2>/dev/null | grep -q "${THEME_NAME}"; then
        update-alternatives --remove default.plymouth "${PLYMOUTH_FILE}" 2>/dev/null || true
        echo "Plymouth alternative removed."
    fi
    
    # Check if any theme is active
    CURRENT_THEME=$(update-alternatives --query default.plymouth 2>/dev/null | grep "^Value:" | cut -d' ' -f2 || echo "")
    
    # If no theme is active, try setting a default
    if [ -z "${CURRENT_THEME}" ]; then
        echo "No active Plymouth theme found, setting default..."
        
    # Try to find and register available themes
        for THEME_PATH in /usr/share/plymouth/themes/*/*.plymouth; do
            if [ -f "${THEME_PATH}" ]; then
                THEME_NAME_FOUND=$(basename "${THEME_PATH}" .plymouth)
                
                # Skip the Monroe theme
                if [ "${THEME_NAME_FOUND}" = "monroe" ]; then
                    continue
                fi
                
                # Register the theme if it's not already registered
                if ! update-alternatives --list default.plymouth 2>/dev/null | grep -q "${THEME_PATH}"; then
                    echo "Registering theme: ${THEME_NAME_FOUND}"
                    update-alternatives --install \
                        /usr/share/plymouth/themes/default.plymouth \
                        default.plymouth \
                        "${THEME_PATH}" \
                        50 || true
                fi
                
                # Prefer the 'spinner' or 'bgrt' themes
                if [ "${THEME_NAME_FOUND}" = "spinner" ] || [ "${THEME_NAME_FOUND}" = "bgrt" ]; then
                    echo "Setting ${THEME_NAME_FOUND} as default theme..."
                    update-alternatives --set default.plymouth "${THEME_PATH}" || true
                    break
                fi
            fi
        done
        
        # If there's still no active theme, choose auto
        CURRENT_THEME=$(update-alternatives --query default.plymouth 2>/dev/null | grep "^Value:" | cut -d' ' -f2 || echo "")
        if [ -z "${CURRENT_THEME}" ]; then
            echo "Letting update-alternatives choose default Plymouth theme..."
            update-alternatives --auto default.plymouth 2>/dev/null || true
        fi
    fi
    
    # Update initramfs
    echo "Updating initramfs..."
    update-initramfs -u
    
    echo "Monroe Plymouth theme removed successfully!"
    
    # Show the current theme
    FINAL_THEME=$(update-alternatives --query default.plymouth 2>/dev/null | grep "^Value:" | cut -d' ' -f2 || echo "none")
    echo "Active Plymouth theme: ${FINAL_THEME}"
}

# Check if running as root
if [ "$EUID" -ne 0 ]; then 
    echo "This script must be run as root"
    exit 1
fi

cleanup_plymouth_theme
