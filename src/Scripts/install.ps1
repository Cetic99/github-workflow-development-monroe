$scriptPath = $PSScriptRoot

$libFolder = Join-Path $scriptPath "lib"
$outputPath = Join-Path $scriptPath "monroe-app"


if (Test-Path $outputPath) {
    Remove-Item -Path $outputPath -Recurse -Force
}

New-Item -ItemType Directory -Path $outputPath | Out-Null

Write-Host "Extracting all zip archives from the lib folder into app folder..."
Get-ChildItem -Path $libFolder -Filter "*.zip" | ForEach-Object {
    $zipFileName = [System.IO.Path]::GetFileNameWithoutExtension($_.Name)
    $zipDestinationPath = Join-Path $outputPath $zipFileName

    if (Test-Path $zipDestinationPath) {
        Remove-Item -Path $zipDestinationPath -Recurse -Force
    }

    Write-Host "Extracting $($_.Name) to $zipDestinationPath..."
    Expand-Archive -Path $_.FullName -DestinationPath $zipDestinationPath -Force
}

$firebirdSysPassword = "oefhueis879fj89HOF8j"

# Check if fileexists
$firebirdBinPath = "C:\Program Files\Firebird\Firebird_5_0\isql.exe"
if (-Not (Test-Path $firebirdBinPath)) {
    Write-Host "Firebird is not installed. Proceeding with installation..."
    Write-Host "Installing Firebird silently..."
    $firebirdInstaller = Join-Path $libFolder "Firebird-5.0.2.1613-0-windows-x64.exe"
    Start-Process -FilePath $firebirdInstaller -ArgumentList "/SP- /SILENT /SUPRESSMSGBOXES /NOCANCEL /NORESTART /MERGETASKS=`"UseSuperServerTask\UseGuardianTask,MenuGroupTask,CopyFbClientAsGds32Task`" /SYSDBAPASSWORD=`"$firebirdSysPassword`" /FORCE" -Wait
} else {
    Write-Host "Firebird is already installed. Skipping installation."
}


Write-Host "Copying db file to app folder..."
$databaseFile = Join-Path $outputPath "MONROE-DATABASE.fdb"
Copy-Item -Path (Join-Path $libFolder "MONROE-DATABASE.fdb") -Destination $databaseFile -Force


Write-Host "Updating backbone configuration file..."
$backboneConfigFilePath = Join-Path -Path $outputPath -ChildPath "backbone" | Join-Path -ChildPath "appsettings.json"
$backboneConfigPlaceholderValue = "`"CashVaultDatabase`": `"<CashVaultDatabase>`""
if ($IsWindows -or $ENV:OS) {
    Write-Host "Windows detected, replacing backslashes in the database file path..."
    $databaseFile = $databaseFile -replace '\\', '\\'
}
$backboneConfigNewValue = "`"CashVaultDatabase`": `"User=SYSDBA;Password=$firebirdSysPassword;Database=localhost:$databaseFile;`""
(Get-Content -Path $backboneConfigFilePath) -replace $backboneConfigPlaceholderValue, $backboneConfigNewValue | Set-Content -Path $backboneConfigFilePath


Write-Host "Updating frontend configuration file..."
$frontendConfigFilePath = Join-Path -Path $outputPath -ChildPath "frontend"| Join-Path -ChildPath "resources" | Join-Path -ChildPath "app.asar.unpacked" | Join-Path -ChildPath "resources" | Join-Path -ChildPath "settings.json"
$frontendConfigPlaceholderValue = "`"logsPath`": `"<logsPath>`""
$backboneLogsFolder = Join-Path -Path $outputPath -ChildPath "backbone" | Join-Path -ChildPath "logs"
if ($IsWindows -or $ENV:OS) {
    Write-Host "Windows detected, replacing backslashes in the logs file path..."
    $backboneLogsFolder = $backboneLogsFolder -replace '\\', '\\'
}
$frontendConfigNewValue = "`"logsPath`": `"$backboneLogsFolder`""
(Get-Content -Path $frontendConfigFilePath) -replace $frontendConfigPlaceholderValue, $frontendConfigNewValue | Set-Content -Path $frontendConfigFilePath



$desktopPath = [Environment]::GetFolderPath("Desktop")

Write-Host "Creating desktop shortcut for backbone..."
$backboneWshShell = New-Object -COMObject WScript.Shell
$backboneExe = Join-Path -Path $outputPath -ChildPath "backbone" | Join-Path -ChildPath "CashVault.WebAPI.exe"
$backboneShortcutPath = Join-Path $desktopPath "monroe-backbone.lnk"
$backboneShortcut = $backboneWshShell.CreateShortcut("$backboneShortcutPath")
$backboneShortcut.TargetPath = $backboneExe
$backboneShortcut.WorkingDirectory = Join-Path -Path $outputPath -ChildPath "backbone"
$backboneShortcut.Save()

Write-Host "Creating desktop shortcut for frontend..."
$frontendWshShell = New-Object -COMObject WScript.Shell
$frontendExe = Join-Path -Path $outputPath -ChildPath "frontend" | Join-Path -ChildPath "cashvault-frontend.exe"
$frontendShortcutPath = Join-Path $desktopPath "monroe-frontend.lnk"
$frontendShortcut = $frontendWshShell.CreateShortcut("$frontendShortcutPath")
$frontendShortcut.TargetPath = $frontendExe
$frontendShortcut.WorkingDirectory = Join-Path -Path $outputPath -ChildPath "frontend"
$frontendShortcut.Save()

Write-Output "Disabling edge swipe gestures..."

$regPath = "HKLM:\SOFTWARE\Policies\Microsoft\Windows\EdgeUI"
$keyName = "AllowEdgeSwipe"

if (-Not (Test-Path $regPath)) {
    New-Item -Path $regPath -Force | Out-Null
    Write-Output "Created registry path: $regPath"
}

Set-ItemProperty -Path $regPath -Name $keyName -Value 0 -Type DWord -Force
Write-Output "Registry value set to disable edge swipe."

$confirm = Get-ItemProperty -Path $regPath -Name $keyName
if ($confirm.AllowEdgeSwipe -eq 0) {
    Write-Output "Swipe gesture is now disabled (AllowEdgeSwipe = 0)."
} else {
    Write-Output "Failed to disable swipe gesture. Current value: $($confirm.AllowEdgeSwipe)"
}

Write-Host "End of installation..."