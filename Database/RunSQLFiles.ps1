$sqlFiles = @(
    "CreateTables.sql",
    "InsertTestData.sql",
    "CleanData.sql",
    "UpdateExistingUsuarios.sql",
    "UpdateUsuariosTable.sql",
    "TestConnection.sql",
    "TestConnection_New.sql",
    "ConsultarFactura_Anulacion.sql"
)

foreach ($file in $sqlFiles) {
    $path = Join-Path $PSScriptRoot $file
    Write-Host "Running $file..."
    sqlcmd -S "10.10.1.248\sqlexpress" -d "SIOS" -U "sa" -P "gs73136" -i $path
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Error running $file"
        break
    }
    Write-Host "$file completed successfully."
}