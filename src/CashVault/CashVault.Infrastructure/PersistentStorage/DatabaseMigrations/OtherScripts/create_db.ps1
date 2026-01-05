# Define database path and name
$DBPath = "C:\Projects\cash-vault\firebird-local-dbs\cashvault-database.fdb"



# Define the path to the isql command line tool
$isql_cmd = "C:\Program Files\Firebird\Firebird_5_0\isql"

# Define the Firebird user and password
$FBUser = "sysdba"
$FBPassword = "masterkey"

# Check if the database already exists
if (Test-Path $DBPath) {
    Write-Host "Database already exists at $DBPath"
    Write-Host "Existing database will be deleted."

    # Remove the existing database file
    Remove-Item $DBPath -Force
    Write-Host "Existing database file deleted. Creating a new database."
}

$sqlCommands = @"
CREATE DATABASE '$DBPath' USER '$FBUser' PASSWORD '$FBPassword';
"@
# Use echo to pass the SQL commands to isql
$sqlCommands | & $isql_cmd

# Optionally, check the exit code
if ($LASTEXITCODE -eq 0) {
    Write-Host "SQL commands executed successfully."
} else {
    Write-Host "Error executing SQL commands. Exit code: $LASTEXITCODE"
}
