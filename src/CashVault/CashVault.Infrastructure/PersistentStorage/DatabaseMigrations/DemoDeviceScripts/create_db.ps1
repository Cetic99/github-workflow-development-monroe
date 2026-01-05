# Define database path and name
$DBPath = "C:\Users\123\Desktop\CashVault\firebird-local-dbs\test-database.fdb"



# Define the path to the isql command line tool
$isql_cmd = "C:\Program Files\Firebird\Firebird_5_0\isql"

# Define the Firebird user and password
$FBUser = "sysdba"
$FBPassword = "masterkey"

# Check if the database already exists
if (Test-Path $DBPath) {
    Write-Host "Database already exists at $DBPath"
} else {

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
}