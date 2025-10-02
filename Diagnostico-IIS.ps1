# Script para diagnosticar y solucionar problemas de IIS con ASP.NET Core

Write-Host "=== DIAGNOSTICO DE IIS PARA ASP.NET CORE ===" -ForegroundColor Cyan

# 1. Verificar si IIS está instalado
Write-Host "`n1. Verificando IIS..." -ForegroundColor Yellow
$iisFeatures = Get-WindowsOptionalFeature -Online | Where-Object { $_.FeatureName -like "*IIS*" -and $_.State -eq "Enabled" }
if ($iisFeatures.Count -gt 0) {
    Write-Host "✓ IIS está instalado" -ForegroundColor Green
} else {
    Write-Host "✗ IIS no está instalado completamente" -ForegroundColor Red
}

# 2. Verificar ASP.NET Core Hosting Bundle
Write-Host "`n2. Verificando ASP.NET Core Hosting Bundle..." -ForegroundColor Yellow
$hostingBundle = Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\IIS\Components\" -ErrorAction SilentlyContinue
if ($hostingBundle) {
    Write-Host "✓ ASP.NET Core Hosting Bundle detectado" -ForegroundColor Green
} else {
    Write-Host "✗ ASP.NET Core Hosting Bundle NO detectado" -ForegroundColor Red
    Write-Host "Descárgalo desde: https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-9.0.0-windows-hosting-bundle-installer" -ForegroundColor Yellow
}

# 3. Verificar servicios de IIS
Write-Host "`n3. Verificando servicios de IIS..." -ForegroundColor Yellow
$w3svc = Get-Service W3SVC -ErrorAction SilentlyContinue
if ($w3svc.Status -eq "Running") {
    Write-Host "✓ Servicio W3SVC está ejecutándose" -ForegroundColor Green
} else {
    Write-Host "✗ Servicio W3SVC no está ejecutándose" -ForegroundColor Red
}

# 4. Verificar Application Pool
Write-Host "`n4. Verificando Application Pool..." -ForegroundColor Yellow
Import-Module WebAdministration -ErrorAction SilentlyContinue
$appPool = Get-IISAppPool -Name "Highdmin" -ErrorAction SilentlyContinue
if ($appPool) {
    Write-Host "✓ Application Pool 'Highdmin' existe" -ForegroundColor Green
    Write-Host "  Estado: $($appPool.State)" -ForegroundColor Gray
    Write-Host "  Versión .NET CLR: $($appPool.ManagedRuntimeVersion)" -ForegroundColor Gray
} else {
    Write-Host "✗ Application Pool 'Highdmin' no existe" -ForegroundColor Red
}

# 5. Verificar Sitio Web
Write-Host "`n5. Verificando Sitio Web..." -ForegroundColor Yellow
$site = Get-IISSite -Name "Highdmin" -ErrorAction SilentlyContinue
if ($site) {
    Write-Host "✓ Sitio Web 'Highdmin' existe" -ForegroundColor Green
    Write-Host "  Estado: $($site.State)" -ForegroundColor Gray
    Write-Host "  Ruta física: $($site.Applications[0].VirtualDirectories[0].PhysicalPath)" -ForegroundColor Gray
} else {
    Write-Host "✗ Sitio Web 'Highdmin' no existe" -ForegroundColor Red
}

# 6. Verificar permisos
Write-Host "`n6. Verificando permisos..." -ForegroundColor Yellow
$publishPath = "D:\INNOVATIONANALYTICS\Anulacion Facturas\publish"
if (Test-Path $publishPath) {
    $acl = Get-Acl $publishPath
    $iisUsers = $acl.Access | Where-Object { $_.IdentityReference -like "*IIS*" }
    if ($iisUsers) {
        Write-Host "✓ Usuario IIS tiene permisos en la carpeta" -ForegroundColor Green
    } else {
        Write-Host "✗ Usuario IIS NO tiene permisos en la carpeta" -ForegroundColor Red
    }
} else {
    Write-Host "✗ La carpeta de publicación no existe: $publishPath" -ForegroundColor Red
}

Write-Host "`n=== SOLUCIONES RECOMENDADAS ===" -ForegroundColor Cyan
Write-Host "1. Instalar ASP.NET Core Hosting Bundle si no está instalado" -ForegroundColor White
Write-Host "2. Reiniciar IIS: iisreset" -ForegroundColor White
Write-Host "3. Verificar que el puerto 80 no esté ocupado" -ForegroundColor White
Write-Host "4. Revisar logs de IIS en C:\inetpub\logs\LogFiles\" -ForegroundColor White
Write-Host "5. Verificar conectividad a la base de datos" -ForegroundColor White

Write-Host "`n¿Deseas ejecutar las correcciones automáticas? (S/N): " -ForegroundColor Yellow -NoNewline
$response = Read-Host

if ($response -eq "S" -or $response -eq "s") {
    Write-Host "`n=== EJECUTANDO CORRECCIONES ===" -ForegroundColor Green

    # Reiniciar IIS
    Write-Host "Reiniciando IIS..." -ForegroundColor Yellow
    iisreset

    # Crear Application Pool si no existe
    if (-not $appPool) {
        Write-Host "Creando Application Pool..." -ForegroundColor Yellow
        New-WebAppPool -Name "Highdmin" -Force
        Set-ItemProperty IIS:\AppPools\Highdmin -Name processModel.identityType -Value ApplicationPoolIdentity
    }

    # Crear sitio web si no existe
    if (-not $site) {
        Write-Host "Creando sitio web..." -ForegroundColor Yellow
        New-Website -Name "Highdmin" -PhysicalPath $publishPath -ApplicationPool "Highdmin" -Port 80 -Force
    }

    # Configurar permisos
    Write-Host "Configurando permisos..." -ForegroundColor Yellow
    $acl = Get-Acl $publishPath
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS","FullControl","ContainerInherit,ObjectInherit","None","Allow")
    $acl.SetAccessRule($accessRule)
    Set-Acl -Path $publishPath -AclObject $acl

    Write-Host "`n✓ Correcciones aplicadas. Reinicia IIS nuevamente si es necesario." -ForegroundColor Green
}

Write-Host "`n=== LOGS DE LA APLICACIÓN ===" -ForegroundColor Cyan
$logPath = "$publishPath\logs"
if (Test-Path $logPath) {
    Write-Host "Logs disponibles en: $logPath" -ForegroundColor White
    Get-ChildItem $logPath -ErrorAction SilentlyContinue | ForEach-Object {
        Write-Host "  $($_.Name)" -ForegroundColor Gray
    }
} else {
    Write-Host "No se encontraron logs. Revisa la configuración de logging en web.config" -ForegroundColor Yellow
}