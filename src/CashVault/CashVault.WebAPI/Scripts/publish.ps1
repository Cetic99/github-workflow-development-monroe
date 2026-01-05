$scriptPath = $PSScriptRoot

$parentDir = Split-Path -Path $scriptPath -Parent
$projectPath = $parentDir

$outputPath = Join-Path $projectPath "publish"
$csprojPath = Join-Path $projectPath "CashVault.WebAPI.csproj"

# Delete output folder if it exists
if (Test-Path $outputPath) {
    Remove-Item -Path $outputPath -Recurse -Force
}

# Create output folder
New-Item -ItemType Directory -Path $outputPath | Out-Null

$runtime = "win-x64"
$configuration = "Release"

dotnet publish `
    $csprojPath `
    -c $configuration `
    -r $runtime `
    --self-contained true `
    -o $outputPath

Write-Host "`nPublished files:"
Get-ChildItem -Path $outputPath


Write-Host "`nPress Enter to exit..."
Read-Host