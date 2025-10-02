-- Script SQL para crear las tablas del sistema de anulación de facturas
-- Base de datos: SIOS
-- Esquema: AnulacionFacturas

USE SIOS;
GO

-- Crear esquema si no existe
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'AnulacionFacturas')
BEGIN
    EXEC('CREATE SCHEMA AnulacionFacturas')
END
GO

-- Crear tabla Pacientes
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Pacientes' AND xtype='U' AND uid = SCHEMA_ID('AnulacionFacturas'))
CREATE TABLE AnulacionFacturas.Pacientes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NumeroIdentificacion NVARCHAR(20) NOT NULL UNIQUE,
    TipoIdentificacion NVARCHAR(10) NULL,
    NombreCompleto NVARCHAR(200) NOT NULL,
    FechaCreacion DATETIME2 DEFAULT GETDATE()
);

-- Crear índices para Pacientes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Pacientes_NumeroIdentificacion' AND object_id = OBJECT_ID('AnulacionFacturas.Pacientes'))
CREATE UNIQUE INDEX IX_Pacientes_NumeroIdentificacion ON AnulacionFacturas.Pacientes (NumeroIdentificacion);

-- Crear tabla Peticiones
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Peticiones' AND xtype='U' AND uid = SCHEMA_ID('AnulacionFacturas'))
CREATE TABLE AnulacionFacturas.Peticiones (
    Id NVARCHAR(20) PRIMARY KEY,
    Asunto NVARCHAR(500) NOT NULL,
    Motivo NVARCHAR(1000) NULL,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    Prioridad INT NOT NULL, -- 0: Baja, 1: Media, 2: Alta
    Estado INT NOT NULL, -- 0: Pendiente, 1: Aprobada, 2: Rechazada
    NumeroAprobacion NVARCHAR(50) NULL,
    CreadoPor NVARCHAR(100) NOT NULL
);

-- Crear índices para Peticiones
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Peticiones_Estado' AND object_id = OBJECT_ID('AnulacionFacturas.Peticiones'))
CREATE INDEX IX_Peticiones_Estado ON AnulacionFacturas.Peticiones (Estado);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Peticiones_FechaCreacion' AND object_id = OBJECT_ID('AnulacionFacturas.Peticiones'))
CREATE INDEX IX_Peticiones_FechaCreacion ON AnulacionFacturas.Peticiones (FechaCreacion);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Peticiones_CreadoPor' AND object_id = OBJECT_ID('AnulacionFacturas.Peticiones'))
CREATE INDEX IX_Peticiones_CreadoPor ON AnulacionFacturas.Peticiones (CreadoPor);

-- Crear tabla Facturas
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Facturas' AND xtype='U' AND uid = SCHEMA_ID('AnulacionFacturas'))
CREATE TABLE AnulacionFacturas.Facturas (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Numero NVARCHAR(50) NOT NULL,
    FechaElaboracion DATETIME2 NOT NULL,
    Valor DECIMAL(18,2) NOT NULL,
    Eps NVARCHAR(100) NOT NULL,
    PacienteId INT NOT NULL,
    FechaRadicacion DATETIME2 NOT NULL,
    PeticionId NVARCHAR(20) NOT NULL,
    FOREIGN KEY (PacienteId) REFERENCES AnulacionFacturas.Pacientes(Id),
    FOREIGN KEY (PeticionId) REFERENCES AnulacionFacturas.Peticiones(Id) ON DELETE CASCADE
);

-- Crear índices para Facturas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Facturas_Numero' AND object_id = OBJECT_ID('AnulacionFacturas.Facturas'))
CREATE INDEX IX_Facturas_Numero ON AnulacionFacturas.Facturas (Numero);

-- Crear tabla Comentarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Comentarios' AND xtype='U' AND uid = SCHEMA_ID('AnulacionFacturas'))
CREATE TABLE AnulacionFacturas.Comentarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Autor NVARCHAR(100) NOT NULL,
    Texto NVARCHAR(1000) NOT NULL,
    Fecha DATETIME2 NOT NULL DEFAULT GETDATE(),
    PeticionId NVARCHAR(20) NOT NULL,
    FOREIGN KEY (PeticionId) REFERENCES AnulacionFacturas.Peticiones(Id) ON DELETE CASCADE
);

-- Crear tabla DocumentosSoporte (para almacenar nombres de archivos)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DocumentosSoporte' AND xtype='U' AND uid = SCHEMA_ID('AnulacionFacturas'))
CREATE TABLE AnulacionFacturas.DocumentosSoporte (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreArchivo NVARCHAR(255) NOT NULL,
    PeticionId NVARCHAR(20) NOT NULL,
    FechaSubida DATETIME2 NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (PeticionId) REFERENCES AnulacionFacturas.Peticiones(Id) ON DELETE CASCADE
);

-- Crear tabla Usuarios
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Usuarios' AND xtype='U' AND uid = SCHEMA_ID('AnulacionFacturas'))
CREATE TABLE AnulacionFacturas.Usuarios (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    NombreCompleto NVARCHAR(200) NOT NULL,
    Rol NVARCHAR(50) NOT NULL DEFAULT 'Usuario',
    Activo BIT NOT NULL DEFAULT 1,
    FechaCreacion DATETIME2 NOT NULL DEFAULT GETDATE(),
    UltimoLogin DATETIME2 NULL
);

-- Crear índices para Usuarios
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_NombreUsuario' AND object_id = OBJECT_ID('AnulacionFacturas.Usuarios'))
CREATE UNIQUE INDEX IX_Usuarios_NombreUsuario ON AnulacionFacturas.Usuarios (NombreUsuario);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Usuarios_Email' AND object_id = OBJECT_ID('AnulacionFacturas.Usuarios'))
CREATE UNIQUE INDEX IX_Usuarios_Email ON AnulacionFacturas.Usuarios (Email);

PRINT 'Tablas del sistema de anulación de facturas creadas exitosamente en el esquema AnulacionFacturas.';
GO