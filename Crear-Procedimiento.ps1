# Script para crear el procedimiento almacenado ConsultarFactura
# Ejecutar este script para crear el procedimiento en la base de datos

Write-Host "=== Creando Procedimiento Almacenado ConsultarFactura ===" -ForegroundColor Green
Write-Host ""

# Parámetros de conexión
$server = "10.10.1.248\sqlexpress"
$database = "SIOS"
$user = "sa"
$password = "gs73136"

# Ruta del archivo SQL
$sqlFile = Join-Path $PSScriptRoot "Database\ConsultarFactura_Anulacion.sql"

# Verificar que el archivo existe
if (!(Test-Path $sqlFile)) {
    Write-Host "Error: No se encuentra el archivo $sqlFile" -ForegroundColor Red
    exit 1
}

Write-Host "Archivo SQL encontrado: $sqlFile" -ForegroundColor Green
Write-Host "Conectando a la base de datos..." -ForegroundColor Yellow

try {
    # Ejecutar el script SQL
    $result = Invoke-Sqlcmd -ServerInstance $server -Database $database -Username $user -Password $password -InputFile $sqlFile -ErrorAction Stop

    Write-Host "✅ Procedimiento almacenado creado exitosamente!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Procedimiento creado: AnulacionFacturas.ConsultarFactura" -ForegroundColor Cyan
    Write-Host "Puedes probarlo ejecutando:" -ForegroundColor White
    Write-Host "EXEC AnulacionFacturas.ConsultarFactura @NumeroFactura = 'TU_NUMERO_FACTURA'" -ForegroundColor White

} catch {
    Write-Host "❌ Error al crear el procedimiento almacenado:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host ""
    Write-Host "Posibles soluciones:" -ForegroundColor Yellow
    Write-Host "1. Verifica que la base de datos SIOS existe" -ForegroundColor White
    Write-Host "2. Verifica que el usuario 'sa' tenga permisos para crear procedimientos" -ForegroundColor White
    Write-Host "3. Verifica que el servidor SQL esté ejecutándose" -ForegroundColor White
    Write-Host "4. Revisa la conexión de red al servidor 10.10.1.248" -ForegroundColor White
}

Write-Host ""
Write-Host "Presiona cualquier tecla para continuar..."
$null = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")