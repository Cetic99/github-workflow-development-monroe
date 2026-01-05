# 🚀 NFC Card Tool - Deployment Guide

## 📦 Release Package

Release folder contains everything needed to run NFC Card Tool on Windows.

### 📁 Package Contents:

```
release/
├── NFC Card Configurator.exe       1.49 MB - Main executable
├── NFC Card Configurator.pdb      25 KB   - Debug symbols (optional)
├── README.md    2.5 KB  - Release documentation
├── Start NFC Card Tool.bat         1.2 KB  - Batch launcher
├── Start-NFCCardTool.ps1           2.5 KB  - PowerShell launcher
└── Assets/
    └── monroe_logo.jpg        118 KB   - Application logo
```

### 📋 Prerequisites:

#### Required:
- ✅ **Windows 10/11** (64-bit)
- ✅ **.NET 8.0 Desktop Runtime** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- ✅ **NFC Reader** (PC/SC compatible: ACR122U, ACR1252U, etc.)

#### Optional but Recommended:
- ✅ **NFC Reader Drivers** (usually auto-installed by Windows)
- ✅ **Administrator privileges** (for Smart Card service management)

### 🎯 Quick Start Options:

#### Option 1: Batch File (Recommended for most users)
```batch
Double-click: "Start NFC Card Tool.bat"
```
- ✅ Checks .NET installation
- ✅ Checks/Starts Smart Card service
- ✅ Launches application

#### Option 2: PowerShell Script (Advanced users)
```powershell
Right-click "Start-NFCCardTool.ps1" → Run with PowerShell
```
- ✅ Colored output
- ✅ Better error handling
- ✅ More detailed checks

#### Option 3: Direct Launch
```
Double-click: "NFC Card Configurator.exe"
```
- Direct execution
- Manual troubleshooting required if issues occur

### 🔧 Post-Deployment Checklist:

1. **Verify .NET Runtime:**
   ```powershell
   dotnet --list-runtimes
 ```
   Should show: `Microsoft.WindowsDesktop.App 8.x.x`

2. **Check Smart Card Service:**
   - Press `Win + R`
   - Type: `services.msc`
   - Find: "Smart Card"
   - Status should be: "Running"

3. **Test NFC Reader:**
   - Connect reader via USB
   - Open Device Manager (`devmgmt.msc`)
   - Check under "Smart card readers"

### 📤 Distribution:

#### For Internal Distribution:
1. **Zip the entire `release` folder**
   ```powershell
   Compress-Archive -Path "release\*" -DestinationPath "NFC_Card_Tool_v1.0.zip"
   ```

2. **Share via:**
   - Network drive
   - Email (if under size limit)
   - Internal file sharing system

#### For External Distribution:
1. Consider creating an installer (e.g., using Inno Setup, WiX)
2. Include .NET 8.0 Runtime in installer
3. Sign the executable with code signing certificate
4. Create proper uninstaller

### 🛡️ Security Notes:

#### Code Signing (Recommended for production):
```powershell
# Sign the executable
signtool sign /f "certificate.pfx" /p "password" "NFC Card Configurator.exe"
```

#### Windows SmartScreen:
- Unsigned apps will show SmartScreen warning
- Users need to click "More info" → "Run anyway"
- Code signing certificate resolves this

### 🔄 Update Process:

1. Build new version:
   ```bash
   dotnet publish -c Release -o release
   ```

2. Copy Assets folder:
   ```powershell
   Copy-Item -Path "NFC card tool\Assets" -Destination "release\Assets" -Recurse -Force
 ```

3. Update version information in README.md

4. Test on clean machine

### 📊 Build Information:

- **Build Type**: Framework-Dependent
- **Configuration**: Release
- **Target Framework**: net8.0-windows
- **Runtime Identifier**: win-x64 (portable)
- **Single File**: Yes
- **Self-Contained**: No (requires .NET 8.0 Runtime)
- **Ready to Run**: No
- **Trimmed**: No

### 🎨 Application Features:

✅ **NFC Reader Management**
- Auto-detect PC/SC readers
- Support for multiple reader types
- Real-time connection status

✅ **Card Operations**
- Read UID and card data
- Write GUID data to cards
- NTAG213/215/216 support

✅ **Debug Console**
- Built-in debug panel (toggle with 🐛 button)
- Real-time operation logging
- Teal/cyan color scheme

✅ **User Interface**
- Modern WPF design
- Teal/cyan color palette
- Intuitive workflow

### 💾 Alternative Build Options:

#### Self-Contained Build (includes .NET runtime):
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o release-standalone
```
- **Pros**: No .NET installation needed
- **Cons**: Larger file size (~70-100 MB)

#### Trimmed Build (smaller size):
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -o release-trimmed
```
- **Pros**: Smaller size
- **Cons**: May break reflection-based features

### 📝 Troubleshooting Common Issues:

#### Issue: "Unable to load DLL 'winscard.dll'"
**Solution**: Install NFC reader drivers

#### Issue: "Application doesn't start"
**Solution**: Install .NET 8.0 Desktop Runtime

#### Issue: "No readers found"
**Solution**: 
1. Check USB connection
2. Check Device Manager
3. Start Smart Card service

#### Issue: "Access Denied" errors
**Solution**: Run as Administrator

### 🌐 Additional Resources:

- [.NET 8.0 Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PC/SC Workgroup](https://pcscworkgroup.com/)
- [NFC Tools](https://www.wakdev.com/en/apps/nfc-tools-pc-mac.html)

---

**Last Updated**: December 2024  
**Build Version**: Release 1.0  
**Target Platform**: Windows x64  
**Framework**: .NET 8.0
