# Script PowerShell para configurar la base de datos del sistema de anulación de facturas
# Requisitos: SQL Server Management Studio (SSMS) o sqlcmd instalado
# Esquema: AnulacionFacturas

param(
    [string]$ServerInstance = "10.10.1.248\sqlexpress",
    [string]$Database = "SIOS",
    [string]$Username = "sa",
    [string]$Password = "gs73136",
    [string]$Schema = "AnulacionFacturas",
    [switch]$CleanData
)

Write-Host "=== Configuración de Base de Datos - Sistema de Anulación de Facturas ===" -ForegroundColor Green
Write-Host "Esquema: $Schema" -ForegroundColor Cyan
Write-Host ""

# Función para ejecutar script SQL
function Execute-SqlScript {
    param(
        [string]$ScriptPath,
        [string]$Description
    )

    if (Test-Path $ScriptPath) {
        Write-Host "Ejecutando: $Description" -ForegroundColor Yellow
        Write-Host "Archivo: $ScriptPath" -ForegroundColor Gray

        try {
            $connectionString = "Server=$ServerInstance;Database=$Database;User Id=$Username;Password=$Password;TrustServerCertificate=True;"

            # Usar sqlcmd para ejecutar el script
            $result = & sqlcmd -S $ServerInstance -d $Database -U $Username -P $Password -i $ScriptPath -I

            if ($LASTEXITCODE -eq 0) {
                Write-Host "[OK] $Description completado exitosamente" -ForegroundColor Green
            } else {
                Write-Host "[ERROR] Error ejecutando $Description" -ForegroundColor Red
                Write-Host "Codigo de salida: $LASTEXITCODE" -ForegroundColor Red
            }
        }
        catch {
            Write-Host "[ERROR] Error: $($_.Exception.Message)" -ForegroundColor Red
        }
    } else {
        Write-Host "[ERROR] Archivo no encontrado: $ScriptPath" -ForegroundColor Red
    }

    Write-Host ""
}

# Obtener la ruta del directorio del script
$scriptDirectory = Split-Path -Parent $MyInvocation.MyCommand.Path
$databaseDirectory = Join-Path $scriptDirectory "Database"

Write-Host "Directorio de scripts: $databaseDirectory" -ForegroundColor Cyan
Write-Host ""

# Verificar conexión a la base de datos
Write-Host "Verificando conexión a la base de datos..." -ForegroundColor Yellow
try {
    $connectionString = "Server=$ServerInstance;Database=$Database;User Id=$Username;Password=$Password;TrustServerCertificate=True;"
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $connectionString
    $connection.Open()
    Write-Host "[OK] Conexion exitosa a $Database en $ServerInstance" -ForegroundColor Green
    $connection.Close()
}
catch {
    Write-Host "[ERROR] Error de conexion: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Verifique que:" -ForegroundColor Yellow
    Write-Host "  - El servidor SQL Server esté ejecutándose" -ForegroundColor Yellow
    Write-Host "  - Las credenciales sean correctas" -ForegroundColor Yellow
    Write-Host "  - La base de datos '$Database' exista" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Ejecutar scripts en orden
if ($CleanData) {
    Execute-SqlScript -ScriptPath (Join-Path $databaseDirectory "CleanData.sql") -Description "Limpieza de datos existentes"
}

Execute-SqlScript -ScriptPath (Join-Path $databaseDirectory "CreateTables.sql") -Description "Creación de tablas"
Execute-SqlScript -ScriptPath (Join-Path $databaseDirectory "InsertTestData.sql") -Description "Inserción de datos de prueba"
Execute-SqlScript -ScriptPath (Join-Path $databaseDirectory "TestConnection_New.sql") -Description "Verificación de estructura"

Write-Host "=== Configuración completada ===" -ForegroundColor Green
Write-Host ""
Write-Host "Para probar la aplicación ASP.NET Core:" -ForegroundColor Cyan
Write-Host "1. Abra Visual Studio Code en el directorio del proyecto" -ForegroundColor White
Write-Host "2. Ejecute: dotnet run" -ForegroundColor White
Write-Host "3. Abra el navegador en: https://localhost:5001" -ForegroundColor White
Write-Host "4. Navegue a: /Peticiones/Recibidas" -ForegroundColor White
Write-Host ""
Write-Host "Notas importantes:" -ForegroundColor Yellow
Write-Host "- Las tablas se crearon en el esquema: $Schema" -ForegroundColor White
Write-Host "- Para limpiar datos existentes use: .\Setup-Database.ps1 -CleanData" -ForegroundColor White
Write-Host ""
Write-Host "La aplicacion deberia mostrar las peticiones de prueba!" -ForegroundColor Green