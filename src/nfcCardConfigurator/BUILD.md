# 🚀 NFC Card Tool - Build Instructions

Quick guide for building Release version of the application.

---

## 📦 Quick Build (One Command)

### Windows Batch:
```batch
build-release.bat
```

### PowerShell:
```powershell
.\build-release.ps1
```

**That's it!** The scripts will:
1. ✅ Clean previous builds
2. ✅ Restore NuGet packages
3. ✅ Build Release configuration
4. ✅ Publish to `release/` folder
5. ✅ Copy Assets

---

## 🔧 Manual Build Steps

If you prefer manual control:

```bash
# Step 1: Clean
dotnet clean "NFC card tool\NFC card tool.csproj" --configuration Release

# Step 2: Restore packages
dotnet restore "NFC card tool\NFC card tool.csproj"

# Step 3: Build
dotnet build "NFC card tool\NFC card tool.csproj" --configuration Release

# Step 4: Publish
dotnet publish "NFC card tool\NFC card tool.csproj" --configuration Release --output "release" --self-contained false -p:PublishSingleFile=true

# Step 5: Copy Assets
xcopy /E /I /Y "NFC card tool\Assets" "release\Assets"
```

---

## 📁 Output Location

After build, find your release package in:
```
📂 release/
├── NFC Card Configurator.exe  (Main application)
├── Start NFC Card Tool.bat    (Launcher)
├── README.md        (User guide)
├── VERSION.md                 (Version info)
└── Assets/          (Resources)
```

---

## 🎯 Distribution

To distribute the application:

1. **Zip the release folder:**
   ```powershell
   Compress-Archive -Path "release\*" -DestinationPath "NFC_Card_Tool_v1.0.zip"
   ```

2. **Share with users**

3. **Users need:**
   - Windows 10/11 (64-bit)
   - .NET 8.0 Desktop Runtime ([Download](https://dotnet.microsoft.com/download/dotnet/8.0))
   - NFC Reader (ACR122U, ACR1252U, etc.)

---

## 🔄 Self-Contained Build (Optional)

For a version that includes .NET runtime (larger file, no runtime needed on target):

```bash
dotnet publish "NFC card tool\NFC card tool.csproj" ^
  --configuration Release ^
  --output "release-standalone" ^
  --runtime win-x64 ^
  --self-contained true ^
  -p:PublishSingleFile=true
```

**Result:** ~70-100 MB executable with all dependencies

---

## ⚙️ Requirements

### For Building:
- ✅ .NET 8.0 SDK
- ✅ Windows 10/11
- ✅ Visual Studio 2022 or VS Code (optional)

### For Running:
- ✅ .NET 8.0 Desktop Runtime
- ✅ NFC Reader with drivers
- ✅ Smart Card service (Windows)

---

## 📝 Version Management

Update version in `NFC card tool.csproj`:

```xml
<Version>1.0.0</Version>
```

Then rebuild with scripts.

---

## 🐛 Troubleshooting

### Build fails with package errors:
```bash
dotnet restore --force
```

### Clean doesn't work:
```bash
rmdir /S /Q "NFC card tool\bin"
rmdir /S /Q "NFC card tool\obj"
rmdir /S /Q "release"
```

### Assets not copying:
Check that `Assets` folder exists in `NFC card tool\Assets`

---

## 📚 Documentation

- **User Guide**: `release\README.md`
- **Version Info**: `release\VERSION.md`
- **Debug Guide**: `DEBUG_README.md`
- **Deployment**: `DEPLOYMENT.md`

---

**Happy Building!** 🎉
