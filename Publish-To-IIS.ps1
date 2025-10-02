# Script completo para publicar Anulación de Facturas en IIS
# Ejecutar como Administrador

param(
    [string]$SiteName = "AnulacionFacturas",
    [string]$AppPoolName = "AnulacionFacturasPool",
    [string]$PublishPath = "D:\IIS\AnulacionFacturas",
    [string]$Port = "8080"
)

Write-Host "=== Publicación de Anulación de Facturas en IIS ===" -ForegroundColor Green
Write-Host ""

# Verificar permisos de administrador
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "ERROR: Este script requiere permisos de administrador." -ForegroundColor Red
    Write-Host "Ejecute PowerShell como administrador y vuelva a intentarlo." -ForegroundColor Yellow
    exit 1
}

# Verificar si IIS está instalado
Write-Host "Verificando instalación de IIS..." -ForegroundColor Yellow
try {
    $iisFeatures = Get-WindowsOptionalFeature -Online | Where-Object { $_.FeatureName -like "*IIS*" -and $_.State -eq "Enabled" }
    if ($iisFeatures.Count -eq 0) {
        Write-Host "ERROR: IIS no está instalado. Instale IIS primero." -ForegroundColor Red
        Write-Host "Ejecute: Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole, IIS-WebServer, IIS-CommonHttpFeatures, IIS-HttpErrors, IIS-HttpLogging, IIS-RequestFiltering, IIS-StaticContent, IIS-WebServerManagementTools, IIS-ApplicationInit, IIS-ISAPIExtensions, IIS-ISAPIFilter, IIS-NetFxExtensibility, IIS-ASPNET, IIS-NetFxExtensibility45, IIS-ASPNET45" -ForegroundColor Yellow
        exit 1
    }
    Write-Host "IIS está instalado correctamente." -ForegroundColor Green
} catch {
    Write-Host "ERROR: No se pudo verificar IIS: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Verificar si ASP.NET Core Hosting Bundle está instalado
Write-Host "Verificando ASP.NET Core Hosting Bundle..." -ForegroundColor Yellow
$hostingBundlePath = "${env:ProgramFiles}\IIS\Asp.Net Core Module\V2\aspnetcorev2.dll"
if (!(Test-Path $hostingBundlePath)) {
    Write-Host "ERROR: ASP.NET Core Hosting Bundle no está instalado." -ForegroundColor Red
    Write-Host "Descargue e instale desde: https://dotnet.microsoft.com/download/dotnet/thank-you/runtime-aspnetcore-9.0.0-windows-hosting-bundle-installer" -ForegroundColor Yellow
    exit 1
}
Write-Host "ASP.NET Core Hosting Bundle está instalado." -ForegroundColor Green

# Importar módulo IIS
Write-Host "Importando módulo IIS..." -ForegroundColor Yellow
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "Módulo IIS cargado correctamente." -ForegroundColor Green
} catch {
    Write-Host "ERROR: No se pudo cargar el módulo WebAdministration: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Establecer directorio de trabajo
$projectPath = "D:\INNOVATIONANALYTICS\Anulacion Facturas"
if (!(Test-Path $projectPath)) {
    Write-Host "ERROR: No se encuentra el directorio del proyecto: $projectPath" -ForegroundColor Red
    exit 1
}

Set-Location $projectPath

# Crear directorio de publicación
Write-Host "Creando directorio de publicación: $PublishPath" -ForegroundColor Yellow
try {
    if (!(Test-Path $PublishPath)) {
        New-Item -ItemType Directory -Path $PublishPath -Force
        Write-Host "Directorio de publicación creado." -ForegroundColor Green
    } else {
        Write-Host "Directorio de publicación ya existe." -ForegroundColor Green
    }
} catch {
    Write-Host "ERROR: No se pudo crear el directorio de publicación: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Publicar aplicación
Write-Host "Publicando aplicación..." -ForegroundColor Yellow
try {
    $projectFile = Join-Path $projectPath "Highdmin.csproj"
    if (!(Test-Path $projectFile)) {
        Write-Host "ERROR: No se encuentra el archivo de proyecto: $projectFile" -ForegroundColor Red
        exit 1
    }

    & dotnet publish $projectFile --configuration Release --output $PublishPath --runtime win-x64 --self-contained false

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Falló la publicación de la aplicación." -ForegroundColor Red
        exit 1
    }
    Write-Host "Aplicación publicada correctamente." -ForegroundColor Green
} catch {
    Write-Host "ERROR: No se pudo publicar la aplicación: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Crear Application Pool
Write-Host "Creando Application Pool: $AppPoolName" -ForegroundColor Yellow
try {
    if (!(Test-Path "IIS:\AppPools\$AppPoolName")) {
        New-WebAppPool -Name $AppPoolName -Force
        Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
        Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name processModel.idleTimeout -Value "00:00:00"
        Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name recycling.periodicRestart.time -Value "00:00:00"
        Write-Host "Application Pool creado correctamente." -ForegroundColor Green
    } else {
        Write-Host "Application Pool ya existe." -ForegroundColor Green
    }
} catch {
    Write-Host "ERROR: No se pudo crear/configurar el Application Pool: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Crear sitio web
Write-Host "Creando sitio web: $SiteName" -ForegroundColor Yellow
try {
    if (!(Test-Path "IIS:\Sites\$SiteName")) {
        New-Website -Name $SiteName -PhysicalPath $PublishPath -ApplicationPool $AppPoolName -Port $Port -Force
        Write-Host "Sitio web creado correctamente en el puerto $Port." -ForegroundColor Green
    } else {
        Write-Host "Sitio web ya existe. Actualizando configuración..." -ForegroundColor Yellow
        Set-ItemProperty "IIS:\Sites\$SiteName" -Name physicalPath -Value $PublishPath
        Set-ItemProperty "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName
        Write-Host "Sitio web actualizado correctamente." -ForegroundColor Green
    }
} catch {
    Write-Host "ERROR: No se pudo crear/configurar el sitio web: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Configurar permisos
Write-Host "Configurando permisos..." -ForegroundColor Yellow
try {
    $acl = Get-Acl $PublishPath
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    Set-Acl -Path $PublishPath -AclObject $acl
    Write-Host "Permisos configurados correctamente para IIS_IUSRS." -ForegroundColor Green
} catch {
    Write-Host "ADVERTENCIA: No se pudieron configurar permisos automáticamente: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Configure manualmente los permisos para 'IIS_IUSRS' en la carpeta $PublishPath" -ForegroundColor Yellow
}

# Verificar estado del sitio web
Write-Host "Verificando estado del sitio web..." -ForegroundColor Yellow
try {
    $site = Get-Website -Name $SiteName
    if ($site.State -eq "Started") {
        Write-Host "Sitio web está ejecutándose correctamente." -ForegroundColor Green
    } else {
        Write-Host "Iniciando sitio web..." -ForegroundColor Yellow
        Start-Website -Name $SiteName
        Write-Host "Sitio web iniciado." -ForegroundColor Green
    }
} catch {
    Write-Host "ADVERTENCIA: No se pudo verificar el estado del sitio web: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Verificar estado del Application Pool
Write-Host "Verificando estado del Application Pool..." -ForegroundColor Yellow
try {
    $appPoolState = Get-WebAppPoolState -Name $AppPoolName
    if ($appPoolState.Value -eq "Started") {
        Write-Host "Application Pool está ejecutándose correctamente." -ForegroundColor Green
    } else {
        Write-Host "Iniciando Application Pool..." -ForegroundColor Yellow
        Start-WebAppPool -Name $AppPoolName
        Write-Host "Application Pool iniciado." -ForegroundColor Green
    }
} catch {
    Write-Host "ADVERTENCIA: No se pudo verificar el estado del Application Pool: $($_.Exception.Message)" -ForegroundColor Yellow
}

# Verificar sitio
Write-Host "Verificando sitio web..." -ForegroundColor Yellow
try {
    Start-Process "http://localhost:$Port"
    Write-Host "Navegador abierto en http://localhost:$Port" -ForegroundColor Green
} catch {
    Write-Host "ADVERTENCIA: No se pudo abrir el navegador automáticamente." -ForegroundColor Yellow
    Write-Host "Acceda manualmente a: http://localhost:$Port" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== INSTALACIÓN COMPLETADA ===" -ForegroundColor Green
Write-Host ""
Write-Host "Sitio web configurado:" -ForegroundColor Cyan
Write-Host "  URL: http://localhost:$Port" -ForegroundColor White
Write-Host "  Directorio: $PublishPath" -ForegroundColor White
Write-Host "  Application Pool: $AppPoolName" -ForegroundColor White
Write-Host ""
Write-Host "Credenciales de prueba:" -ForegroundColor Yellow
Write-Host "  Usuario: admin" -ForegroundColor White
Write-Host "  Contraseña: admin123" -ForegroundColor White
Write-Host ""
Write-Host "Notas importantes:" -ForegroundColor Yellow
Write-Host "1. Asegúrese de que LocalDB esté instalado y configurado" -ForegroundColor White
Write-Host "2. Verifique que el firewall permita conexiones al puerto $Port" -ForegroundColor White
Write-Host "3. Si hay problemas de base de datos, ejecute Setup-LocalDB.ps1" -ForegroundColor White
Write-Host "4. Los logs de aplicación están en el Visor de Eventos de Windows" -ForegroundColor White
Write-Host ""
Write-Host "¡La aplicación está lista para usar!" -ForegroundColor Green

# Función de limpieza
function Cleanup {
    Write-Host ""
    Write-Host "Realizando limpieza..." -ForegroundColor Yellow
    try {
        # Limpiar archivos temporales si existen
        $tempFiles = @(
            "$env:TEMP\dotnet-publish-*",
            "$env:TEMP\iis-config-*"
        )

        foreach ($tempFile in $tempFiles) {
            if (Test-Path $tempFile) {
                Remove-Item $tempFile -Force -ErrorAction SilentlyContinue
            }
        }

        Write-Host "Limpieza completada." -ForegroundColor Green
    } catch {
        Write-Host "ADVERTENCIA: Error durante la limpieza: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}

# Registrar función de limpieza
$null = Register-EngineEvent PowerShell.Exiting -Action ${function:Cleanup}