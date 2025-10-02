# Script para diagnosticar y resolver problemas de conexión SQL Server
# Ejecutar como Administrador

Write-Host "=== Diagnóstico de Conexión SQL Server ===" -ForegroundColor Green
Write-Host ""

# Verificar si LocalDB está disponible
Write-Host "1. Verificando LocalDB..." -ForegroundColor Yellow
try {
    $localDbInstances = & sqllocaldb info
    if ($localDbInstances) {
        Write-Host "LocalDB encontrado. Instancias:" -ForegroundColor Green
        $localDbInstances | ForEach-Object { Write-Host "  - $_" -ForegroundColor White }
    } else {
        Write-Host "LocalDB no encontrado o no hay instancias." -ForegroundColor Yellow
    }
} catch {
    Write-Host "LocalDB no está instalado." -ForegroundColor Red
}

# Verificar SQL Server Browser
Write-Host "`n2. Verificando servicios SQL Server..." -ForegroundColor Yellow
$services = Get-Service | Where-Object { $_.Name -like "*SQL*" -or $_.DisplayName -like "*SQL*" }
if ($services) {
    Write-Host "Servicios SQL encontrados:" -ForegroundColor Green
    $services | ForEach-Object {
        $status = if ($_.Status -eq "Running") { "Running" } else { "Stopped" }
        $color = if ($_.Status -eq "Running") { "Green" } else { "Red" }
        Write-Host "  - $($_.DisplayName) ($($_.Name)): " -NoNewline
        Write-Host $status -ForegroundColor $color
    }
} else {
    Write-Host "No se encontraron servicios SQL." -ForegroundColor Yellow
}

# Probar conexión al servidor remoto
Write-Host "`n3. Probando conexión al servidor remoto..." -ForegroundColor Yellow
$remoteServer = "10.10.1.248"
$remotePort = "1433"

try {
    $tcpClient = New-Object System.Net.Sockets.TcpClient
    $connectResult = $tcpClient.BeginConnect($remoteServer, $remotePort, $null, $null)
    $waitResult = $connectResult.AsyncWaitHandle.WaitOne(5000, $false)

    if ($waitResult -and $tcpClient.Connected) {
        Write-Host "Conexión exitosa al servidor remoto $remoteServer en puerto $remotePort" -ForegroundColor Green
        $tcpClient.Close()
    } else {
        Write-Host "No se puede conectar al servidor remoto $remoteServer en puerto $remotePort" -ForegroundColor Red
        Write-Host "Posibles causas:" -ForegroundColor Yellow
        Write-Host "  - El servidor no está ejecutándose" -ForegroundColor White
        Write-Host "  - Firewall bloqueando la conexión" -ForegroundColor White
        Write-Host "  - Red no accesible" -ForegroundColor White
    }
} catch {
    Write-Host "Error al probar conexión: $($_.Exception.Message)" -ForegroundColor Red
}

# Verificar configuración de red
Write-Host "`n4. Verificando configuración de red..." -ForegroundColor Yellow
try {
    $networkConfig = Get-NetAdapter | Where-Object { $_.Status -eq "Up" } | Select-Object Name, Status, MacAddress
    Write-Host "Adaptadores de red activos:" -ForegroundColor Green
    $networkConfig | ForEach-Object {
        Write-Host "  - $($_.Name): $($_.Status)" -ForegroundColor White
    }
} catch {
    Write-Host "Error al verificar configuración de red: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host "`n=== SOLUCIONES RECOMENDADAS ===" -ForegroundColor Cyan
Write-Host ""

Write-Host "Opción 1: Usar LocalDB (Recomendado para desarrollo)" -ForegroundColor Yellow
Write-Host "1. Instalar SQL Server LocalDB si no está instalado" -ForegroundColor White
Write-Host "2. Ejecutar: .\Setup-LocalDB.ps1" -ForegroundColor White
Write-Host "3. Cambiar la cadena de conexión en appsettings.json:" -ForegroundColor White
Write-Host '   "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AnulacionFacturas;Trusted_Connection=True;"' -ForegroundColor Gray
Write-Host ""

Write-Host "Opción 2: Verificar servidor remoto" -ForegroundColor Yellow
Write-Host "1. Verificar que SQL Server esté ejecutándose en 10.10.1.248" -ForegroundColor White
Write-Host "2. Verificar que el puerto 1433 esté abierto" -ForegroundColor White
Write-Host "3. Verificar credenciales de conexión (sa/gs73136)" -ForegroundColor White
Write-Host "4. Verificar configuración de red y firewall" -ForegroundColor White
Write-Host ""

Write-Host "Opción 3: Usar SQL Server Express local" -ForegroundColor Yellow
Write-Host "1. Instalar SQL Server Express local" -ForegroundColor White
Write-Host "2. Cambiar cadena de conexión a:" -ForegroundColor White
Write-Host '   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=AnulacionFacturas;Trusted_Connection=True;"' -ForegroundColor Gray
Write-Host ""

Write-Host "Comando para probar conexión:" -ForegroundColor Green
Write-Host 'sqlcmd -S "(localdb)\mssqllocaldb" -Q "SELECT @@VERSION"' -ForegroundColor White
Write-Host ""

Write-Host "Una vez resuelto el problema de conexión, ejecutar:" -ForegroundColor Green
Write-Host ".\Publish-To-IIS.ps1" -ForegroundColor White