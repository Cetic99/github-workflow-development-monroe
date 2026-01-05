$scriptPath = $PSScriptRoot

$parentDir = Split-Path -Path $scriptPath -Parent
$projectPath = $parentDir

# Variables
$npmScript = "build:win"

# Check if Node.js and npm are installed
if (-not (Get-Command "npm" -ErrorAction SilentlyContinue)) {
    Write-Host "Error: npm is not installed or not added to PATH." -ForegroundColor Red
    exit 1
}

npm install --prefix $projectPath

Write-Host "`nRunning npm script '$npmScript'..."
npm run $npmScript --prefix $projectPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nError: npm script '$npmScript' failed." -ForegroundColor Red
    exit 1
}

Write-Host "`nPress Enter to exit..."
Read-Host
