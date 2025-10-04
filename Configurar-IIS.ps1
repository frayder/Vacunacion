# Script para configurar IIS automáticamente para ASP.NET Core

Write-Host "=== CONFIGURACIÓN AUTOMÁTICA DE IIS ===" -ForegroundColor Cyan

$siteName = "Vacunacion"
$publishPath = "D:\INNOVATIONANALYTICS\Anulacion Facturas\publish"
$appPoolName = "Vacunacion"
$port = 80

# Verificar si estamos ejecutando como administrador
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "✗ Este script debe ejecutarse como administrador" -ForegroundColor Red
    Write-Host "Ejecuta PowerShell como administrador y vuelve a intentarlo" -ForegroundColor Yellow
    exit
}

Write-Host "Configurando IIS para $siteName..." -ForegroundColor Yellow

# Importar módulo de IIS
Import-Module WebAdministration -ErrorAction SilentlyContinue

# 1. Crear Application Pool
Write-Host "1. Creando Application Pool..." -ForegroundColor White
if (Get-IISAppPool -Name $appPoolName -ErrorAction SilentlyContinue) {
    Write-Host "  Application Pool ya existe, eliminando..." -ForegroundColor Gray
    Remove-WebAppPool -Name $appPoolName -ErrorAction SilentlyContinue
}

New-WebAppPool -Name $appPoolName -Force
Set-ItemProperty IIS:\AppPools\$appPoolName -Name processModel.identityType -Value ApplicationPoolIdentity
Set-ItemProperty IIS:\AppPools\$appPoolName -Name processModel.idleTimeout -Value "00:00:00"
Set-ItemProperty IIS:\AppPools\$appPoolName -Name recycling.periodicRestart.time -Value "00:00:00"
Write-Host "  ✓ Application Pool creado" -ForegroundColor Green

# 2. Crear sitio web
Write-Host "2. Creando sitio web..." -ForegroundColor White
if (Get-IISSite -Name $siteName -ErrorAction SilentlyContinue) {
    Write-Host "  Sitio web ya existe, eliminando..." -ForegroundColor Gray
    Remove-IISSite -Name $siteName -Confirm:$false -ErrorAction SilentlyContinue
}

New-Website -Name $siteName -PhysicalPath $publishPath -ApplicationPool $appPoolName -Port $port -Force
Write-Host "  ✓ Sitio web creado en puerto $port" -ForegroundColor Green

# 3. Configurar permisos
Write-Host "3. Configurando permisos..." -ForegroundColor White
if (Test-Path $publishPath) {
    $acl = Get-Acl $publishPath
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS","FullControl","ContainerInherit,ObjectInherit","None","Allow")
    $acl.SetAccessRule($accessRule)
    Set-Acl -Path $publishPath -AclObject $acl
    Write-Host "  ✓ Permisos configurados" -ForegroundColor Green
} else {
    Write-Host "  ✗ La ruta $publishPath no existe" -ForegroundColor Red
}

# 4. Reiniciar IIS
Write-Host "4. Reiniciando IIS..." -ForegroundColor White
iisreset
Write-Host "  ✓ IIS reiniciado" -ForegroundColor Green

# 5. Verificar configuración
Write-Host "5. Verificando configuración..." -ForegroundColor White
$appPool = Get-IISAppPool -Name $appPoolName
$site = Get-IISSite -Name $siteName

if ($appPool.State -eq "Started") {
    Write-Host "  ✓ Application Pool está ejecutándose" -ForegroundColor Green
} else {
    Write-Host "  ✗ Application Pool no está ejecutándose" -ForegroundColor Red
}

if ($site.State -eq "Started") {
    Write-Host "  ✓ Sitio web está ejecutándose" -ForegroundColor Green
} else {
    Write-Host "  ✗ Sitio web no está ejecutándose" -ForegroundColor Red
}

Write-Host "`n=== CONFIGURACIÓN COMPLETADA ===" -ForegroundColor Cyan
Write-Host "Sitio web: http://localhost:$port" -ForegroundColor White
Write-Host "Application Pool: $appPoolName" -ForegroundColor White
Write-Host "Ruta física: $publishPath" -ForegroundColor White

Write-Host "`n=== PRUEBA DE CONECTIVIDAD ===" -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:$port" -TimeoutSec 10 -ErrorAction Stop
    Write-Host "✓ Sitio web responde correctamente (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "✗ Error al acceder al sitio web: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Revisa los logs de IIS y la configuración de la aplicación" -ForegroundColor Yellow
}

Write-Host "`n=== LOGS DISPONIBLES ===" -ForegroundColor Cyan
$logPath = "$publishPath\logs"
if (Test-Path $logPath) {
    Write-Host "Logs de aplicación: $logPath" -ForegroundColor White
} else {
    Write-Host "No hay logs configurados. Revisa web.config" -ForegroundColor Yellow
}

Write-Host "`nPresiona Enter para continuar..." -ForegroundColor Gray
Read-Host