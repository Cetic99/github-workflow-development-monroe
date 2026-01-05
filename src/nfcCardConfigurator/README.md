# NFC Card Configurator

**Monroe NFC Card Configuration Tool**

A modern, user-friendly Windows application for reading, analyzing, and configuring NFC cards using ACR1252U and compatible PC/SC readers.

Developed by **Monroe**.

## ? Features

### ?? Comprehensive Card Analysis
- **Complete UID Reading** with length detection (Single/Double/Triple size)
- **Card Type Identification** (MIFARE Classic 1K/4K, Ultralight, NTAG, DESFire)
- **ATR (Answer To Reset)** analysis
- **Protocol Detection** (T0/T1)
- **Memory Size Estimation**
- **Factory State Detection** - Identifies if card is blank or contains data
- **Lock Status Check** - Shows if card is write-protected
- **NDEF Support Detection**
- **Block-by-Block Memory Dump** for cards with data
- **Manufacturer Block** information

### ?? Key Capabilities
- **Automatic Card Detection** - Instantly detects when card is placed on reader
- **Real-time Monitoring** - Tracks card presence and removal
- **Detailed Logging** - Debug panel with comprehensive operation logs
- **PC/SC Service Diagnostics** - Built-in troubleshooting tools
- **Modern UI** - Clean, professional interface with dark blue theme

## ?? Requirements

- **Operating System**: Windows 10/11
- **.NET**: .NET 8.0 Runtime
- **Hardware**: ACR1252U, ACR122U, or any PC/SC compatible NFC reader
- **Drivers**: PC/SC smart card drivers (usually pre-installed on Windows)

## ?? Getting Started

### Installation

1. Ensure your NFC reader is connected to your computer
2. Verify that the "Smart Card" service is running in Windows
3. Download and run `NFC Card Configurator.exe`

### Usage

1. **Start Application** - The tool automatically detects connected readers
2. **Select Reader** - Choose your NFC reader from the dropdown
3. **Place Card** - Put your NFC card on the reader
4. **Auto-Connect** - Application automatically establishes connection
5. **Read Card** - Click "Read" button to analyze the card
6. **View Results** - Comprehensive analysis appears in a popup window

## ?? Card Analysis Output

The tool provides detailed information organized in sections:

### IDENTIFICATION
- UID (Unique Identifier)
- UID Length and Type
- Card Type Classification
- Hardware Version (for supported cards)

### TECHNICAL SPECIFICATIONS
- ATR (Answer To Reset)
- Communication Protocol
- Memory Size
- Manufacturer Block Data

### CARD STATUS
- Factory State: YES (blank) or NO (has data)
- Lock Status: LOCKED or UNLOCKED
- NDEF Support: Available or Not Available
- Block Statistics:  Readable, Used, and Empty blocks

### DATA CONTENT
- NDEF Formatted Data (if present)
- Memory Dump of non-empty blocks
- Hex visualization of card data

### SUMMARY
- Quick overview of card state
- Data usage statistics
- NDEF status summary

## ?? Supported Card Types

- **MIFARE Classic** 1K (1024 bytes) / 4K (4096 bytes)
- **MIFARE Ultralight** (512 bits)
- **MIFARE DESFire**
- **NTAG** 213/215/216
- **ISO 14443 Type A** compatible cards

## ??? Troubleshooting

### Reader Not Detected
1. Check USB connection
2. Verify drivers are installed (Device Manager ? Smart card readers)
3. Ensure "Smart Card" service is running (services.msc)
4. Click "PC/SC Diagnostics" button for detailed check

### Card Not Reading
1. Ensure card is properly placed on reader
2. Try removing and placing card again
3. Check Debug Log for detailed error messages
4. Some cards may require authentication (future feature)

## ?? Technical Details

- **Framework**: .NET 8.0 WPF
- **Architecture**: MVVM-inspired design
- **Card Communication**: PC/SC (Personal Computer/Smart Card) via PCSC NuGet package
- **APDU Commands**: Standard ISO 7816-4 commands
- **UI**: Modern WPF with Material Design influences

## ?? Security Note

This tool operates in **read-only mode** for data analysis. The "Write" functionality is reserved for future updates and will include appropriate safety measures.

## ?? License

Copyright © Monroe 2024. All rights reserved.

## ?? Support

For issues, questions, or feature requests, please contact Monroe support team or check the Debug Log for detailed diagnostic information.

## ?? Screenshots

The application features:
- **Clean Interface** with dark blue accent colors
- **Real-time Status Indicators** (green/blue/gray)
- **Comprehensive Debug Panel** with timestamps
- **Professional Card Data Presentation**

---

**Version**: 1.0.0  
**Last Updated**: 2024  
**Developed by**: Monroe

*Monroe NFC Card Configuration Tool for Windows*
