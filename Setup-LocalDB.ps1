# Script para configurar LocalDB y crear la base de datos para IIS
# Requisitos: SQL Server LocalDB instalado

param(
    [string]$DatabaseName = "AnulacionFacturas",
    [string]$DataPath = "$env:USERPROFILE\AnulacionFacturas_Data"
)

Write-Host "=== Configuración de LocalDB para Anulación de Facturas ===" -ForegroundColor Green

# Crear directorio para datos si no existe
if (!(Test-Path $DataPath)) {
    New-Item -ItemType Directory -Path $DataPath -Force
    Write-Host "Directorio de datos creado: $DataPath" -ForegroundColor Yellow
}

# Verificar si LocalDB está instalado
try {
    $localDbVersion = & sqllocaldb versions
    Write-Host "LocalDB encontrado. Versiones disponibles:" -ForegroundColor Green
    $localDbVersion | ForEach-Object { Write-Host "  $_" -ForegroundColor Gray }
} catch {
    Write-Host "ERROR: LocalDB no está instalado. Instale SQL Server Express LocalDB primero." -ForegroundColor Red
    Write-Host "Descargue desde: https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb" -ForegroundColor Yellow
    exit 1
}

# Crear instancia de LocalDB si no existe
$instanceName = "AnulacionFacturasInstance"
try {
    $instances = & sqllocaldb info
    if ($instances -notcontains $instanceName) {
        Write-Host "Creando instancia LocalDB: $instanceName" -ForegroundColor Yellow
        & sqllocaldb create $instanceName
        & sqllocaldb start $instanceName
    } else {
        Write-Host "Instancia LocalDB ya existe: $instanceName" -ForegroundColor Green
        & sqllocaldb start $instanceName
    }
} catch {
    Write-Host "ERROR: No se pudo crear/iniciar la instancia LocalDB: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Obtener la cadena de conexión
$connectionString = "(localdb)\$instanceName"

Write-Host "Instancia LocalDB configurada: $connectionString" -ForegroundColor Green
Write-Host "Base de datos será creada automáticamente por Entity Framework" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White
Write-Host "Para usar con IIS:" -ForegroundColor Yellow
Write-Host "1. Actualice appsettings.Production.json con la cadena de conexión correcta" -ForegroundColor White
Write-Host "2. Asegúrese de que el pool de aplicaciones en IIS tenga permisos para acceder a LocalDB" -ForegroundColor White
Write-Host "3. El usuario del pool de aplicaciones debe tener permisos en la carpeta de datos" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "Configuración completada exitosamente!" -ForegroundColor Green