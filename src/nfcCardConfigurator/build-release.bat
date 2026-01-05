@echo off
REM ========================================================
REM   NFC Card Tool - Release Build Script
REM   Builds and packages the application for distribution
REM ========================================================

echo.
echo ========================================================
echo   NFC Card Tool - Release Build
echo ========================================================
echo.

REM Step 1: Clean previous builds
echo [1/5] Cleaning previous build...
dotnet clean "NFC card tool\NFC card tool.csproj" --configuration Release
if errorlevel 1 (
    echo ERROR: Clean failed!
    pause
    exit /b 1
)
echo [OK] Clean completed
echo.

REM Step 2: Restore NuGet packages
echo [2/5] Restoring NuGet packages...
dotnet restore "NFC card tool\NFC card tool.csproj"
if errorlevel 1 (
    echo ERROR: Restore failed!
    pause
  exit /b 1
)
echo [OK] Packages restored
echo.

REM Step 3: Build Release version
echo [3/5] Building Release configuration...
dotnet build "NFC card tool\NFC card tool.csproj" --configuration Release --no-restore
if errorlevel 1 (
    echo ERROR: Build failed!
    pause
    exit /b 1
)
echo [OK] Build completed
echo.

REM Step 4: Publish to release folder
echo [4/5] Publishing to release folder...
dotnet publish "NFC card tool\NFC card tool.csproj" --configuration Release --output "release" --self-contained false -p:PublishSingleFile=true
if errorlevel 1 (
    echo ERROR: Publish failed!
    pause
    exit /b 1
)
echo [OK] Published successfully
echo.

REM Step 5: Copy Assets
echo [5/5] Copying Assets folder...
xcopy /E /I /Y "NFC card tool\Assets" "release\Assets"
if errorlevel 1 (
    echo WARNING: Assets copy failed!
) else (
    echo [OK] Assets copied
)
echo.

echo ========================================================
echo   BUILD COMPLETED SUCCESSFULLY!
echo ========================================================
echo.
echo Release package location: %CD%\release\
echo Main executable: NFC Card Configurator.exe
echo.
echo You can now distribute the contents of the release folder.
echo.
pause
