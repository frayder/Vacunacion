-- Script completo para configurar el módulo de Pacientes
-- Ejecutar este script en la base de datos Highdmin

USE [Highdmin]
GO

PRINT 'Iniciando configuración del módulo de Pacientes...'
GO

-- 1. Crear tabla Pacientes
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Pacientes' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Pacientes] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Eps] nvarchar(100) NOT NULL,
        [Identificacion] nvarchar(20) NOT NULL,
        [Nombres] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NOT NULL,
        [FechaNacimiento] datetime2(7) NOT NULL,
        [Sexo] nvarchar(10) NOT NULL,
        [Telefono] nvarchar(15) NULL,
        [Email] nvarchar(255) NULL,
        [Direccion] nvarchar(500) NULL,
        [Estado] bit NOT NULL DEFAULT 1,
        [FechaCreacion] datetime2(7) NOT NULL DEFAULT GETDATE(),
        [FechaActualizacion] datetime2(7) NULL,
        CONSTRAINT [PK_Pacientes] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
    
    -- Crear índices únicos
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Pacientes_Identificacion] ON [dbo].[Pacientes]([Identificacion] ASC)
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Pacientes_Email] ON [dbo].[Pacientes]([Email] ASC) WHERE [Email] IS NOT NULL
    
    PRINT 'Tabla Pacientes creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla Pacientes ya existe.'
END
GO

-- 2. Insertar datos de ejemplo
IF NOT EXISTS (SELECT * FROM [dbo].[Pacientes])
BEGIN
    INSERT INTO [dbo].[Pacientes] 
    ([Eps], [Identificacion], [Nombres], [Apellidos], [FechaNacimiento], [Sexo], [Telefono], [Email], [Direccion], [Estado])
    VALUES 
    ('EPS SURA', '12345678', 'Juan Carlos', 'Pérez López', '1990-01-15', 'M', '3001234567', 'juan.perez@email.com', 'Calle 123 #45-67', 1),
    ('NUEVA EPS', '87654321', 'María José', 'González García', '1985-05-20', 'F', '3007654321', 'maria.gonzalez@email.com', 'Carrera 45 #12-34', 1),
    ('SANITAS', '11223344', 'Carlos Alberto', 'Rodríguez Martín', '1992-11-08', 'M', '3009876543', 'carlos.rodriguez@email.com', 'Avenida 80 #23-45', 1),
    ('COMPENSAR', '55667788', 'Ana María', 'Silva Torres', '1988-03-12', 'F', '3005678901', 'ana.silva@email.com', 'Calle 67 #89-01', 1),
    ('COOMEVA', '99887766', 'Luis Fernando', 'Jiménez Ruiz', '1995-07-25', 'M', '3003456789', 'luis.jimenez@email.com', 'Carrera 23 #56-78', 1)
    
    PRINT 'Datos de ejemplo insertados en la tabla Pacientes.'
END
GO

-- 3. Agregar menú de Pacientes
IF NOT EXISTS (SELECT * FROM [dbo].[MenuItems] WHERE [Name] = 'Pacientes')
BEGIN
    INSERT INTO [dbo].[MenuItems] ([Name], [Resource], [Icon], [Controller], [Action], [Order], [IsActive])
    VALUES ('Pacientes', 'Pacientes', 'ri-user-line', 'Pacientes', 'Index', 5, 1)
    
    PRINT 'Menú de Pacientes agregado exitosamente.'
END
ELSE
BEGIN
    PRINT 'El menú de Pacientes ya existe.'
END
GO

-- 4. Agregar permisos básicos para el rol Administrador (si existe)
DECLARE @AdminRoleId INT = (SELECT TOP 1 Id FROM [dbo].[Roles] WHERE Nombre = 'Administrador')
DECLARE @PacientesMenuId INT = (SELECT Id FROM [dbo].[MenuItems] WHERE Name = 'Pacientes')

IF @AdminRoleId IS NOT NULL AND @PacientesMenuId IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM [dbo].[RolePermissions] WHERE RoleId = @AdminRoleId AND MenuItemId = @PacientesMenuId)
    BEGIN
        INSERT INTO [dbo].[RolePermissions] (RoleId, MenuItemId, CanCreate, CanRead, CanUpdate, CanDelete)
        VALUES (@AdminRoleId, @PacientesMenuId, 1, 1, 1, 1)
        
        PRINT 'Permisos de Pacientes asignados al rol Administrador.'
    END
END
GO

PRINT 'Configuración del módulo de Pacientes completada exitosamente.'
PRINT 'Total de pacientes registrados: ' + CAST((SELECT COUNT(*) FROM [dbo].[Pacientes]) AS VARCHAR(10))
GO