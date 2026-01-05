# ========================================================
# NFC Card Tool - Release Build Script (PowerShell)
# Builds and packages the application for distribution
# ========================================================

Write-Host ""
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "  NFC Card Tool - Release Build" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan
Write-Host ""

$ErrorActionPreference = "Stop"
$projectPath = "NFC card tool\NFC card tool.csproj"
$releasePath = "release"

try {
    # Step 1: Clean previous builds
    Write-Host "[1/5] Cleaning previous build..." -ForegroundColor Yellow
    dotnet clean $projectPath --configuration Release
    Write-Host "[OK] Clean completed" -ForegroundColor Green
  Write-Host ""

    # Step 2: Restore NuGet packages
    Write-Host "[2/5] Restoring NuGet packages..." -ForegroundColor Yellow
    dotnet restore $projectPath
  Write-Host "[OK] Packages restored" -ForegroundColor Green
    Write-Host ""

    # Step 3: Build Release version
    Write-Host "[3/5] Building Release configuration..." -ForegroundColor Yellow
    dotnet build $projectPath --configuration Release --no-restore
    Write-Host "[OK] Build completed" -ForegroundColor Green
 Write-Host ""

    # Step 4: Publish to release folder
    Write-Host "[4/5] Publishing to release folder..." -ForegroundColor Yellow
    dotnet publish $projectPath --configuration Release --output $releasePath --self-contained false -p:PublishSingleFile=true
    Write-Host "[OK] Published successfully" -ForegroundColor Green
    Write-Host ""

    # Step 5: Copy Assets
    Write-Host "[5/5] Copying Assets folder..." -ForegroundColor Yellow
    $assetsSource = "NFC card tool\Assets"
    $assetsDestination = "$releasePath\Assets"
    
    if (Test-Path $assetsSource) {
      Copy-Item -Path $assetsSource -Destination $assetsDestination -Recurse -Force
        Write-Host "[OK] Assets copied" -ForegroundColor Green
    } else {
    Write-Host "[WARNING] Assets folder not found" -ForegroundColor Yellow
    }
    Write-Host ""

    # Success message
    Write-Host "========================================================" -ForegroundColor Green
    Write-Host "  BUILD COMPLETED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "========================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Release package location: " -NoNewline
    Write-Host "$((Get-Location).Path)\$releasePath\" -ForegroundColor Cyan
    Write-Host "Main executable: " -NoNewline
    Write-Host "NFC Card Configurator.exe" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "You can now distribute the contents of the release folder." -ForegroundColor White
Write-Host ""

} catch {
    Write-Host ""
    Write-Host "========================================================" -ForegroundColor Red
    Write-Host "  BUILD FAILED!" -ForegroundColor Red
    Write-Host "========================================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
  Write-Host ""
    exit 1
}

Write-Host "Press any key to exit..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
