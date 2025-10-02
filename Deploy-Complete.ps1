param(
    [string]$PublishPath = "D:\IIS\AnulacionFacturas",
    [string]$AppPoolName = "AnulacionFacturasPool",
    [string]$SiteName = "AnulacionFacturas",
    [string]$Port = "8080",
    [string]$AdminUsername = "admin",
    [string]$AdminPassword = "admin123"
)

Write-Host "=== DESPLIEGUE COMPLETO DE ANULACIÓN DE FACTURAS ===" -ForegroundColor Green
Write-Host "Iniciando proceso de despliegue automatizado..." -ForegroundColor Yellow
Write-Host "" -ForegroundColor White

# Paso 1: Configurar LocalDB
Write-Host "PASO 1: Configurando LocalDB..." -ForegroundColor Cyan
try {
    & ".\Setup-LocalDB.ps1"
    Write-Host "✓ LocalDB configurado correctamente" -ForegroundColor Green
} catch {
    Write-Host "ERROR en configuración de LocalDB: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Paso 2: Inicializar base de datos
Write-Host "" -ForegroundColor White
Write-Host "PASO 2: Inicializando base de datos..." -ForegroundColor Cyan
try {
    & ".\Initialize-Database.ps1" -AdminUsername $AdminUsername -AdminPassword $AdminPassword
    Write-Host "✓ Base de datos inicializada correctamente" -ForegroundColor Green
} catch {
    Write-Host "ERROR en inicialización de base de datos: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Paso 3: Desplegar en IIS
Write-Host "" -ForegroundColor White
Write-Host "PASO 3: Desplegando en IIS..." -ForegroundColor Cyan
try {
    & ".\Deploy-To-IIS.ps1" -PublishPath $PublishPath -AppPoolName $AppPoolName -SiteName $SiteName -Port $Port
    Write-Host "✓ Despliegue en IIS completado correctamente" -ForegroundColor Green
} catch {
    Write-Host "ERROR en despliegue IIS: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Paso 4: Verificación final
Write-Host "" -ForegroundColor White
Write-Host "PASO 4: Verificación final..." -ForegroundColor Cyan
try {
    # Probar conexión HTTP
    $response = Invoke-WebRequest -Uri "http://localhost:$Port" -TimeoutSec 30 -ErrorAction Stop
    if ($response.StatusCode -eq 200) {
        Write-Host "✓ Aplicación responde correctamente en http://localhost:$Port" -ForegroundColor Green
    } else {
        Write-Host "⚠ Aplicación responde con código: $($response.StatusCode)" -ForegroundColor Yellow
    }
} catch {
    Write-Host "⚠ No se pudo verificar la aplicación automáticamente: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Verifique manualmente en http://localhost:$Port" -ForegroundColor Yellow
}

Write-Host "" -ForegroundColor White
Write-Host "╔══════════════════════════════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║                    DEPLOYMENT COMPLETADO                   ║" -ForegroundColor Green
Write-Host "╚══════════════════════════════════════════════════════════════╝" -ForegroundColor Green
Write-Host "" -ForegroundColor White
Write-Host "📍 URL de la aplicación: http://localhost:$Port" -ForegroundColor Cyan
Write-Host "👤 Usuario administrador: $AdminUsername" -ForegroundColor Cyan
Write-Host "🔑 Contraseña: $AdminPassword" -ForegroundColor Cyan
Write-Host "🗄️  Base de datos: LocalDB (AnulacionFacturas)" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White
Write-Host "📋 Próximos pasos:" -ForegroundColor Yellow
Write-Host "1. Abra http://localhost:$Port en su navegador" -ForegroundColor White
Write-Host "2. Inicie sesión con las credenciales de administrador" -ForegroundColor White
Write-Host "3. Configure usuarios adicionales si es necesario" -ForegroundColor White
Write-Host "4. Suba archivos Excel de peticiones para procesar" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "🔧 En caso de problemas:" -ForegroundColor Yellow
Write-Host "- Verifique los logs de IIS en el Visor de Eventos" -ForegroundColor White
Write-Host "- Confirme que LocalDB esté ejecutándose: sqllocaldb info" -ForegroundColor White
Write-Host "- Revise la configuración del sitio en IIS Manager" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "¡La aplicación de Anulación de Facturas está lista para usar!" -ForegroundColor Green