-- Script para crear la tabla Pacientes
USE [Highdmin]
GO

-- Crear tabla Pacientes
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

-- Insertar algunos datos de ejemplo
IF NOT EXISTS (SELECT * FROM [dbo].[Pacientes])
BEGIN
    INSERT INTO [dbo].[Pacientes] 
    ([Eps], [Identificacion], [Nombres], [Apellidos], [FechaNacimiento], [Sexo], [Telefono], [Email], [Direccion], [Estado])
    VALUES 
    ('EPS SURA', '12345678', 'Juan Carlos', 'Pérez López', '1990-01-15', 'M', '3001234567', 'juan.perez@email.com', 'Calle 123 #45-67', 1),
    ('NUEVA EPS', '87654321', 'María José', 'González García', '1985-05-20', 'F', '3007654321', 'maria.gonzalez@email.com', 'Carrera 45 #12-34', 1),
    ('SANITAS', '11223344', 'Carlos Alberto', 'Rodríguez Martín', '1992-11-08', 'M', '3009876543', 'carlos.rodriguez@email.com', 'Avenida 80 #23-45', 1)
    
    PRINT 'Datos de ejemplo insertados en la tabla Pacientes.'
END
GO