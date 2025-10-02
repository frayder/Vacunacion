param(
    [string]$PublishPath = "D:\IIS\AnulacionFacturas",
    [string]$AppPoolName = "AnulacionFacturasPool",
    [string]$SiteName = "AnulacionFacturas",
    [string]$Port = "8080"
)

Write-Host "=== Publicación de Anulación de Facturas en IIS ===" -ForegroundColor Green

# Verificar permisos de administrador
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "ERROR: Este script requiere permisos de administrador." -ForegroundColor Red
    Write-Host "Ejecute PowerShell como administrador y vuelva a intentarlo." -ForegroundColor Yellow
    exit 1
}

# Verificar que IIS esté instalado
try {
    Import-Module WebAdministration -ErrorAction Stop
    Write-Host "✓ Módulo IIS cargado correctamente" -ForegroundColor Green
} catch {
    Write-Host "ERROR: IIS no está instalado o el módulo WebAdministration no está disponible." -ForegroundColor Red
    Write-Host "Instale IIS y las características de desarrollo de ASP.NET." -ForegroundColor Yellow
    exit 1
}

# Crear directorio de publicación
if (!(Test-Path $PublishPath)) {
    New-Item -ItemType Directory -Path $PublishPath -Force
    Write-Host "✓ Directorio de publicación creado: $PublishPath" -ForegroundColor Green
}

# Publicar la aplicación
Write-Host "Publicando aplicación..." -ForegroundColor Yellow
try {
    dotnet publish "D:\INNOVATIONANALYTICS\Anulacion Facturas\Highdmin.csproj" -c Release -o $PublishPath --self-contained false
    Write-Host "✓ Aplicación publicada correctamente" -ForegroundColor Green
} catch {
    Write-Host "ERROR: Falló la publicación: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Crear Application Pool
Write-Host "Configurando Application Pool..." -ForegroundColor Yellow
try {
    if (!(Test-Path "IIS:\AppPools\$AppPoolName")) {
        New-WebAppPool -Name $AppPoolName -Force
        Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name processModel.identityType -Value ApplicationPoolIdentity
        Write-Host "✓ Application Pool creado: $AppPoolName" -ForegroundColor Green
    } else {
        Write-Host "✓ Application Pool ya existe: $AppPoolName" -ForegroundColor Green
    }
} catch {
    Write-Host "ERROR: No se pudo crear el Application Pool: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Crear sitio web
Write-Host "Configurando sitio web en IIS..." -ForegroundColor Yellow
try {
    if (!(Test-Path "IIS:\Sites\$SiteName")) {
        New-Website -Name $SiteName -PhysicalPath $PublishPath -ApplicationPool $AppPoolName -Port $Port -Force
        Write-Host "✓ Sitio web creado: $SiteName en puerto $Port" -ForegroundColor Green
    } else {
        Write-Host "✓ Sitio web ya existe: $SiteName" -ForegroundColor Green
        # Actualizar configuración si es necesario
        Set-ItemProperty "IIS:\Sites\$SiteName" -Name physicalPath -Value $PublishPath
        Set-ItemProperty "IIS:\Sites\$SiteName" -Name applicationPool -Value $AppPoolName
    }
} catch {
    Write-Host "ERROR: No se pudo crear el sitio web: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Configurar permisos
Write-Host "Configurando permisos..." -ForegroundColor Yellow
try {
    $acl = Get-Acl $PublishPath
    $accessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    $acl.SetAccessRule($accessRule)
    Set-Acl -Path $PublishPath -AclObject $acl
    Write-Host "✓ Permisos configurados para IIS_IUSRS" -ForegroundColor Green
} catch {
    Write-Host "ADVERTENCIA: No se pudieron configurar permisos automáticamente: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Configure manualmente los permisos para IIS_IUSRS en $PublishPath" -ForegroundColor Yellow
}

# Verificar configuración de LocalDB
Write-Host "Verificando configuración de LocalDB..." -ForegroundColor Yellow
try {
    $localDbStatus = & sqllocaldb info mssqllocaldb 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ LocalDB está disponible" -ForegroundColor Green
    } else {
        Write-Host "⚠ LocalDB no está ejecutándose. Iniciándolo..." -ForegroundColor Yellow
        & sqllocaldb start mssqllocaldb
    }
} catch {
    Write-Host "ADVERTENCIA: No se pudo verificar LocalDB: $($_.Exception.Message)" -ForegroundColor Yellow
    Write-Host "Asegúrese de que LocalDB esté instalado y ejecutándose." -ForegroundColor Yellow
}

# Probar la aplicación
Write-Host "Probando aplicación..." -ForegroundColor Yellow
try {
    Start-Process "http://localhost:$Port"
    Write-Host "✓ Aplicación iniciada en http://localhost:$Port" -ForegroundColor Green
} catch {
    Write-Host "ADVERTENCIA: No se pudo abrir el navegador automáticamente." -ForegroundColor Yellow
}

Write-Host "" -ForegroundColor White
Write-Host "=== DEPLOYMENT COMPLETADO ===" -ForegroundColor Green
Write-Host "Aplicación desplegada en: http://localhost:$Port" -ForegroundColor Cyan
Write-Host "Application Pool: $AppPoolName" -ForegroundColor Cyan
Write-Host "Directorio: $PublishPath" -ForegroundColor Cyan
Write-Host "" -ForegroundColor White
Write-Host "Para acceder a la aplicación:" -ForegroundColor Yellow
Write-Host "1. Abra http://localhost:$Port en su navegador" -ForegroundColor White
Write-Host "2. Use las credenciales de administrador: admin / admin123" -ForegroundColor White
Write-Host "3. La base de datos se creará automáticamente en LocalDB" -ForegroundColor White
Write-Host "" -ForegroundColor White
Write-Host "Si hay problemas:" -ForegroundColor Yellow
Write-Host "- Verifique que LocalDB esté ejecutándose: sqllocaldb info" -ForegroundColor White
Write-Host "- Revise los logs de IIS en el Visor de Eventos" -ForegroundColor White
Write-Host "- Verifique la configuración en IIS Manager" -ForegroundColor White