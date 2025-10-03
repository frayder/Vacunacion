-- ===================================================================
-- Script: CreateRegistroVacunacionTable.sql
-- Descripción: Crear tabla RegistroVacunacion para el sistema de vacunación
-- Fecha: 2025-10-02
-- ===================================================================

USE [HighdminDB]
GO

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RegistroVacunacion' AND xtype='U')
BEGIN
    PRINT 'Creando tabla RegistroVacunacion...'
    
    CREATE TABLE [dbo].[RegistroVacunacion] (
        -- Clave primaria
        [Id] INT IDENTITY(1,1) NOT NULL,
        
        -- Campo consecutivo único
        [Consecutivo] NVARCHAR(20) NOT NULL,
        
        -- DATOS DEL PACIENTE
        [NombresApellidos] NVARCHAR(255) NOT NULL,
        [TipoDocumento] NVARCHAR(10) NOT NULL,
        [NumeroDocumento] NVARCHAR(20) NOT NULL,
        [FechaNacimiento] DATETIME2 NOT NULL,
        [Genero] NVARCHAR(10) NOT NULL,
        [Telefono] NVARCHAR(15) NULL,
        [Direccion] NVARCHAR(500) NULL,
        
        -- DATOS DE AFILIACIÓN (Claves foráneas)
        [AseguradoraId] INT NULL,
        [RegimenAfiliacionId] INT NULL,
        [PertenenciaEtnicaId] INT NULL,
        
        -- DATOS DE ATENCIÓN (Claves foráneas)
        [CentroAtencionId] INT NULL,
        [CondicionUsuariaId] INT NULL,
        [TipoCarnetId] INT NULL,
        
        -- DATOS DE LA VACUNA
        [Vacuna] NVARCHAR(100) NOT NULL,
        [NumeroDosis] NVARCHAR(20) NOT NULL,
        [FechaAplicacion] DATETIME2 NOT NULL,
        [Lote] NVARCHAR(50) NULL,
        [Laboratorio] NVARCHAR(100) NULL,
        [ViaAdministracion] NVARCHAR(50) NULL,
        [SitioAplicacion] NVARCHAR(50) NULL,
        
        -- DATOS DEL RESPONSABLE
        [Vacunador] NVARCHAR(255) NULL,
        [RegistroProfesional] NVARCHAR(50) NULL,
        
        -- OBSERVACIONES Y NOTAS
        [Observaciones] NVARCHAR(1000) NULL,
        [NotasFinales] NVARCHAR(1000) NULL,
        
        -- CAMPOS DE AUDITORÍA
        [Estado] BIT NOT NULL DEFAULT(1),
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT(GETDATE()),
        [FechaModificacion] DATETIME2 NULL,
        [UsuarioCreadorId] INT NULL,
        [UsuarioModificadorId] INT NULL,
        
        -- Definir clave primaria
        CONSTRAINT [PK_RegistroVacunacion] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
    
    PRINT 'Tabla RegistroVacunacion creada exitosamente.'
END
ELSE
BEGIN
    PRINT 'La tabla RegistroVacunacion ya existe.'
END
GO

-- ===================================================================
-- CREAR ÍNDICES PARA MEJORAR RENDIMIENTO
-- ===================================================================

-- Índice único para el consecutivo
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RegistroVacunacion_Consecutivo')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_RegistroVacunacion_Consecutivo] 
    ON [dbo].[RegistroVacunacion] ([Consecutivo] ASC)
    PRINT 'Índice IX_RegistroVacunacion_Consecutivo creado.'
END

-- Índice para número de documento (para búsquedas rápidas)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RegistroVacunacion_NumeroDocumento')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RegistroVacunacion_NumeroDocumento] 
    ON [dbo].[RegistroVacunacion] ([NumeroDocumento] ASC)
    PRINT 'Índice IX_RegistroVacunacion_NumeroDocumento creado.'
END

-- Índice para fecha de aplicación (para reportes por fecha)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RegistroVacunacion_FechaAplicacion')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RegistroVacunacion_FechaAplicacion] 
    ON [dbo].[RegistroVacunacion] ([FechaAplicacion] ASC)
    PRINT 'Índice IX_RegistroVacunacion_FechaAplicacion creado.'
END

-- Índice para estado (para filtrar registros activos)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RegistroVacunacion_Estado')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_RegistroVacunacion_Estado] 
    ON [dbo].[RegistroVacunacion] ([Estado] ASC)
    PRINT 'Índice IX_RegistroVacunacion_Estado creado.'
END

GO

-- ===================================================================
-- CREAR CLAVES FORÁNEAS (solo si las tablas referenciadas existen)
-- ===================================================================

-- Clave foránea para Aseguradora
IF EXISTS (SELECT * FROM sysobjects WHERE name='Aseguradora' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_Aseguradora')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_Aseguradora] 
    FOREIGN KEY ([AseguradoraId]) REFERENCES [dbo].[Aseguradora] ([Id])
    PRINT 'Clave foránea FK_RegistroVacunacion_Aseguradora creada.'
END

-- Clave foránea para RegimenAfiliacion
IF EXISTS (SELECT * FROM sysobjects WHERE name='RegimenAfiliacion' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_RegimenAfiliacion')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_RegimenAfiliacion] 
    FOREIGN KEY ([RegimenAfiliacionId]) REFERENCES [dbo].[RegimenAfiliacion] ([Id])
    PRINT 'Clave foránea FK_RegistroVacunacion_RegimenAfiliacion creada.'
END

-- Clave foránea para PertenenciaEtnica
IF EXISTS (SELECT * FROM sysobjects WHERE name='PertenenciaEtnica' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_PertenenciaEtnica')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_PertenenciaEtnica] 
    FOREIGN KEY ([PertenenciaEtnicaId]) REFERENCES [dbo].[PertenenciaEtnica] ([Id])
    PRINT 'Clave foránea FK_RegistroVacunacion_PertenenciaEtnica creada.'
END

-- Clave foránea para CentroAtencion
IF EXISTS (SELECT * FROM sysobjects WHERE name='CentroAtencion' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_CentroAtencion')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_CentroAtencion] 
    FOREIGN KEY ([CentroAtencionId]) REFERENCES [dbo].[CentroAtencion] ([Id])
    PRINT 'Clave foránea FK_RegistroVacunacion_CentroAtencion creada.'
END

-- Clave foránea para CondicionUsuaria
IF EXISTS (SELECT * FROM sysobjects WHERE name='CondicionUsuaria' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_CondicionUsuaria')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_CondicionUsuaria] 
    FOREIGN KEY ([CondicionUsuariaId]) REFERENCES [dbo].[CondicionUsuaria] ([Id])
    PRINT 'Clave foránea FK_RegistroVacunacion_CondicionUsuaria creada.'
END

-- Clave foránea para TipoCarnet
IF EXISTS (SELECT * FROM sysobjects WHERE name='TipoCarnet' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_TipoCarnet')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_TipoCarnet] 
    FOREIGN KEY ([TipoCarnetId]) REFERENCES [dbo].[TipoCarnet] ([Id])
    PRINT 'Clave foránea FK_RegistroVacunacion_TipoCarnet creada.'
END

-- Clave foránea para Usuario Creador
IF EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_UsuarioCreador')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_UsuarioCreador] 
    FOREIGN KEY ([UsuarioCreadorId]) REFERENCES [dbo].[Users] ([UserId])
    PRINT 'Clave foránea FK_RegistroVacunacion_UsuarioCreador creada.'
END

-- Clave foránea para Usuario Modificador
IF EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
   AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_RegistroVacunacion_UsuarioModificador')
BEGIN
    ALTER TABLE [dbo].[RegistroVacunacion]
    ADD CONSTRAINT [FK_RegistroVacunacion_UsuarioModificador] 
    FOREIGN KEY ([UsuarioModificadorId]) REFERENCES [dbo].[Users] ([UserId])
    PRINT 'Clave foránea FK_RegistroVacunacion_UsuarioModificador creada.'
END

GO

-- ===================================================================
-- VERIFICAR LA CREACIÓN DE LA TABLA
-- ===================================================================

IF EXISTS (SELECT * FROM sysobjects WHERE name='RegistroVacunacion' AND xtype='U')
BEGIN
    PRINT '✓ Tabla RegistroVacunacion creada correctamente.'
    
    -- Mostrar información de la tabla
    SELECT 
        'RegistroVacunacion' as TablaCreada,
        COUNT(*) as TotalColumnas
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'RegistroVacunacion'
    
    -- Mostrar las columnas creadas
    SELECT 
        COLUMN_NAME as Columna,
        DATA_TYPE as TipoDato,
        IS_NULLABLE as PermiteNulos,
        CHARACTER_MAXIMUM_LENGTH as LongitudMaxima
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'RegistroVacunacion'
    ORDER BY ORDINAL_POSITION
END
ELSE
BEGIN
    PRINT '✗ Error: No se pudo crear la tabla RegistroVacunacion.'
END

PRINT '====================================================================='
PRINT 'Script CreateRegistroVacunacionTable.sql ejecutado completamente.'
PRINT '====================================================================='