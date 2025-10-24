param(
    [string]$ConnectionString = "Server=(localdb)\mssqllocaldb;Database=AnulacionFacturas;Trusted_Connection=True;",
    [string]$AdminUsername = "admin",
    [string]$AdminPassword = "admin123",
    [string]$AdminEmail = "admin@anulacionfacturas.com"
)

Write-Host "=== Configuración Inicial de Base de Datos ===" -ForegroundColor Green

# Función para hashear contraseña (simple hash para demo)
function Get-SimpleHash {
    param([string]$password)
    $hash = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($password)
    $hashBytes = $hash.ComputeHash($bytes)
    return [System.BitConverter]::ToString($hashBytes).Replace("-", "").ToLower()
}

$hashedPassword = Get-SimpleHash $AdminPassword

# Script SQL para inicializar la base de datos
$sqlScript = @"
-- Crear esquema si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'AnulacionFacturas')
BEGIN
    EXEC('CREATE SCHEMA [AnulacionFacturas]')
    PRINT 'Esquema AnulacionFacturas creado.'
END

-- Crear tabla de Usuarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' AND xtype='U')
BEGIN
    CREATE TABLE [AnulacionFacturas].[Usuarios] (
        Id INT PRIMARY KEY SERIAL(1,1),
        Username varchar(50) UNIQUE NOT NULL,
        Password varchar(255) NOT NULL,
        Email varchar(100),
        Role varchar(50) DEFAULT 'User',
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        IsActive BIT DEFAULT 1
    )
    PRINT 'Tabla Usuarios creada.'
END

-- Crear tabla de Peticiones
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Peticiones' AND xtype='U')
BEGIN
    CREATE TABLE [AnulacionFacturas].[Peticiones] (
        Id INT PRIMARY KEY SERIAL(1,1),
        NumeroPeticion varchar(50) UNIQUE NOT NULL,
        Estado varchar(50) DEFAULT 'Pendiente',
        FechaCreacion DATETIME2 DEFAULT GETDATE(),
        FechaActualizacion DATETIME2 DEFAULT GETDATE(),
        UsuarioId INT,
        Observaciones varchar(MAX),
        FOREIGN KEY (UsuarioId) REFERENCES [AnulacionFacturas].[Usuarios](Id)
    )
    PRINT 'Tabla Peticiones creada.'
END

-- Crear tabla de Pacientes
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Pacientes' AND xtype='U')
BEGIN
    CREATE TABLE [AnulacionFacturas].[Pacientes] (
        Id INT PRIMARY KEY SERIAL(1,1),
        Nombre varchar(100) NOT NULL,
        Apellido varchar(100) NOT NULL,
        Cedula varchar(20) UNIQUE NOT NULL,
        Telefono varchar(20),
        Email varchar(100),
        FechaNacimiento DATE,
        Direccion varchar(255),
        CreatedAt DATETIME2 DEFAULT GETDATE()
    )
    PRINT 'Tabla Pacientes creada.'
END

-- Crear tabla de Facturas
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Facturas' AND xtype='U')
BEGIN
    CREATE TABLE [AnulacionFacturas].[Facturas] (
        Id INT PRIMARY KEY SERIAL(1,1),
        NumeroFactura varchar(50) UNIQUE NOT NULL,
        PacienteId INT,
        Monto DECIMAL(18,2) NOT NULL,
        FechaEmision DATE NOT NULL,
        FechaVencimiento DATE,
        Estado varchar(50) DEFAULT 'Pendiente',
        Descripcion varchar(MAX),
        CreatedAt DATETIME2 DEFAULT GETDATE(),
        FOREIGN KEY (PacienteId) REFERENCES [AnulacionFacturas].[Pacientes](Id)
    )
    PRINT 'Tabla Facturas creada.'
END

-- Insertar usuario administrador si no existe
IF NOT EXISTS (SELECT * FROM [AnulacionFacturas].[Usuarios] WHERE Username = '$AdminUsername')
BEGIN
    INSERT INTO [AnulacionFacturas].[Usuarios] (Username, Password, Email, Role)
    VALUES ('$AdminUsername', '$hashedPassword', '$AdminEmail', 'Admin')
    PRINT 'Usuario administrador creado: $AdminUsername'
END
ELSE
BEGIN
    PRINT 'Usuario administrador ya existe: $AdminUsername'
END

-- Insertar datos de prueba
IF NOT EXISTS (SELECT * FROM [AnulacionFacturas].[Pacientes] WHERE Cedula = '123456789')
BEGIN
    INSERT INTO [AnulacionFacturas].[Pacientes] (Nombre, Apellido, Cedula, Telefono, Email)
    VALUES ('Juan', 'Pérez', '123456789', '809-555-0123', 'juan.perez@email.com')

    INSERT INTO [AnulacionFacturas].[Pacientes] (Nombre, Apellido, Cedula, Telefono, Email)
    VALUES ('María', 'González', '987654321', '809-555-0456', 'maria.gonzalez@email.com')

    PRINT 'Datos de prueba insertados.'
END

PRINT 'Configuración inicial completada exitosamente.'
"@

# Ejecutar el script SQL
Write-Host "Ejecutando configuración inicial..." -ForegroundColor Yellow

try {
    $sqlScript | Out-File -FilePath "$env:TEMP\init_db.sql" -Encoding UTF8

    # Usar sqlcmd para ejecutar el script
    $result = & sqlcmd -S "(localdb)\mssqllocaldb" -i "$env:TEMP\init_db.sql" -d "AnulacionFacturas" 2>&1

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Configuración inicial completada exitosamente" -ForegroundColor Green
        Write-Host "Usuario administrador: $AdminUsername" -ForegroundColor Cyan
        Write-Host "Contraseña: $AdminPassword" -ForegroundColor Cyan
    } else {
        Write-Host "ERROR: Falló la configuración inicial" -ForegroundColor Red
        Write-Host "Detalles: $result" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}
finally {
    # Limpiar archivo temporal
    Remove-Item "$env:TEMP\init_db.sql" -ErrorAction SilentlyContinue
}

Write-Host "" -ForegroundColor White
Write-Host "=== BASE DE DATOS CONFIGURADA ===" -ForegroundColor Green
Write-Host "La aplicación está lista para usar." -ForegroundColor Cyan