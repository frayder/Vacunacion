-- Script para crear todas las tablas del proyecto de Vacunación
-- Ejecutar este script manualmente en SQL Server Management Studio o Azure Data Studio

-- Verificar si la base de datos existe, si no crearla (opcional)
-- USE [master]
-- GO
-- IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'VacunacionDB')
-- BEGIN
--     CREATE DATABASE [VacunacionDB]
-- END
-- GO
-- USE [VacunacionDB]
-- GO

-- ==============================================
-- TABLAS RBAC (Role-Based Access Control)
-- ==============================================

-- Tabla Users
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Users' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [UserId] INT IDENTITY(1,1) NOT NULL,
        [UserName] NVARCHAR(100) NOT NULL,
        [Email] NVARCHAR(255) NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId]),
        CONSTRAINT [UQ_Users_UserName] UNIQUE ([UserName]),
        CONSTRAINT [UQ_Users_Email] UNIQUE ([Email])
    )
    PRINT 'Tabla Users creada exitosamente'
END
ELSE
    PRINT 'Tabla Users ya existe'
GO

-- Tabla Roles
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Roles' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Roles] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Nombre] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(255) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id])
    )
    PRINT 'Tabla Roles creada exitosamente'
END
ELSE
    PRINT 'Tabla Roles ya existe'
GO

-- Tabla Permissions
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Permissions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Permissions] (
        [PermissionId] INT IDENTITY(1,1) NOT NULL,
        [Resource] NVARCHAR(255) NOT NULL,
        [Action] NVARCHAR(50) NOT NULL,
        CONSTRAINT [PK_Permissions] PRIMARY KEY CLUSTERED ([PermissionId]),
        CONSTRAINT [UQ_Permissions_Resource_Action] UNIQUE ([Resource], [Action])
    )
    PRINT 'Tabla Permissions creada exitosamente'
END
ELSE
    PRINT 'Tabla Permissions ya existe'
GO

-- Tabla MenuItems
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MenuItems' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[MenuItems] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Name] NVARCHAR(100) NOT NULL,
        [Resource] NVARCHAR(255) NOT NULL,
        [Icon] NVARCHAR(50) NULL,
        [Url] NVARCHAR(255) NULL,
        [Controller] NVARCHAR(100) NULL,
        [Action] NVARCHAR(100) NULL,
        [ParentId] INT NULL,
        [Order] INT NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_MenuItems] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_MenuItems_Resource] UNIQUE ([Resource]),
        CONSTRAINT [FK_MenuItems_Parent] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[MenuItems]([Id])
    )
    PRINT 'Tabla MenuItems creada exitosamente'
END
ELSE
    PRINT 'Tabla MenuItems ya existe'
GO

-- Tabla UserRoles (Relación muchos a muchos entre Users y Roles)
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserRoles' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[UserRoles] (
        [UserId] INT NOT NULL,
        [RoleId] INT NOT NULL,
        CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId], [RoleId]),
        CONSTRAINT [FK_UserRoles_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([UserId]) ON DELETE CASCADE,
        CONSTRAINT [FK_UserRoles_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE
    )
    PRINT 'Tabla UserRoles creada exitosamente'
END
ELSE
    PRINT 'Tabla UserRoles ya existe'
GO

-- Tabla RolePermissions
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RolePermissions' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[RolePermissions] (
        [RoleId] INT NOT NULL,
        [MenuItemId] INT NOT NULL,
        [PermissionId] INT NULL,
        [CanCreate] BIT NOT NULL DEFAULT 0,
        [CanRead] BIT NOT NULL DEFAULT 0,
        [CanUpdate] BIT NOT NULL DEFAULT 0,
        [CanDelete] BIT NOT NULL DEFAULT 0,
        [CanActivate] BIT NOT NULL DEFAULT 0,
        [CanResetPassword] BIT NOT NULL DEFAULT 0,
        CONSTRAINT [PK_RolePermissions] PRIMARY KEY CLUSTERED ([RoleId], [MenuItemId]),
        CONSTRAINT [FK_RolePermissions_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_MenuItems] FOREIGN KEY ([MenuItemId]) REFERENCES [dbo].[MenuItems]([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_RolePermissions_Permissions] FOREIGN KEY ([PermissionId]) REFERENCES [dbo].[Permissions]([PermissionId])
    )
    PRINT 'Tabla RolePermissions creada exitosamente'
END
ELSE
    PRINT 'Tabla RolePermissions ya existe'
GO

-- ==============================================
-- TABLAS DE CATÁLOGOS
-- ==============================================

-- Tabla TiposCarnet
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='TiposCarnet' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[TiposCarnet] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_TiposCarnet] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_TiposCarnet_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla TiposCarnet creada exitosamente'
END
ELSE
    PRINT 'Tabla TiposCarnet ya existe'
GO

-- Tabla CondicionesUsuarias
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CondicionesUsuarias' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[CondicionesUsuarias] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_CondicionesUsuarias] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_CondicionesUsuarias_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla CondicionesUsuarias creada exitosamente'
END
ELSE
    PRINT 'Tabla CondicionesUsuarias ya existe'
GO

-- Tabla PertenenciasEtnicas
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PertenenciasEtnicas' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[PertenenciasEtnicas] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_PertenenciasEtnicas] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_PertenenciasEtnicas_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla PertenenciasEtnicas creada exitosamente'
END
ELSE
    PRINT 'Tabla PertenenciasEtnicas ya existe'
GO

-- Tabla Aseguradoras
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Aseguradoras' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Aseguradoras] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Aseguradoras] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Aseguradoras_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla Aseguradoras creada exitosamente'
END
ELSE
    PRINT 'Tabla Aseguradoras ya existe'
GO

-- Tabla RegimenesAfiliacion
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='RegimenesAfiliacion' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[RegimenesAfiliacion] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_RegimenesAfiliacion] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_RegimenesAfiliacion_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla RegimenesAfiliacion creada exitosamente'
END
ELSE
    PRINT 'Tabla RegimenesAfiliacion ya existe'
GO

-- Tabla Hospitales
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Hospitales' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Hospitales] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Hospitales] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Hospitales_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla Hospitales creada exitosamente'
END
ELSE
    PRINT 'Tabla Hospitales ya existe'
GO

-- Tabla CentrosAtencion
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='CentrosAtencion' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[CentrosAtencion] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Tipo] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_CentrosAtencion] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_CentrosAtencion_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla CentrosAtencion creada exitosamente'
END
ELSE
    PRINT 'Tabla CentrosAtencion ya existe'
GO

-- ==============================================
-- TABLAS PRINCIPALES DE VACUNACIÓN
-- ==============================================

-- Tabla Insumos
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Insumos' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Insumos] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Codigo] NVARCHAR(10) NOT NULL,
        [Nombre] NVARCHAR(255) NOT NULL,
        [Tipo] NVARCHAR(50) NOT NULL,
        [Descripcion] NVARCHAR(500) NULL,
        [RangoDosis] NVARCHAR(255) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Insumos] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Insumos_Codigo] UNIQUE ([Codigo])
    )
    PRINT 'Tabla Insumos creada exitosamente'
END
ELSE
    PRINT 'Tabla Insumos ya existe'
GO

-- Tabla Entradas
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Entradas' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Entradas] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [FechaEntrada] DATETIME2 NOT NULL DEFAULT GETDATE(),
        [InsumoId] INT NOT NULL,
        [Cantidad] INT NOT NULL,
        [UsuarioId] INT NOT NULL,
        [Mes] NVARCHAR(255) NOT NULL,
        [Notas] NVARCHAR(500) NULL,
        [Estado] BIT NOT NULL DEFAULT 1,
        [FechaCreacion] DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_Entradas] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [FK_Entradas_Insumos] FOREIGN KEY ([InsumoId]) REFERENCES [dbo].[Insumos]([Id]),
        CONSTRAINT [FK_Entradas_Users] FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[Users]([UserId]),
        CONSTRAINT [CHK_Entradas_Cantidad] CHECK ([Cantidad] > 0)
    )
    PRINT 'Tabla Entradas creada exitosamente'
END
ELSE
    PRINT 'Tabla Entradas ya existe'
GO

-- ==============================================
-- ÍNDICES ADICIONALES PARA MEJORAR RENDIMIENTO
-- ==============================================

-- Índices en tabla Users
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_IsActive')
CREATE INDEX [IX_Users_IsActive] ON [dbo].[Users] ([IsActive])

-- Índices en tabla MenuItems
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_ParentId')
CREATE INDEX [IX_MenuItems_ParentId] ON [dbo].[MenuItems] ([ParentId])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_MenuItems_IsActive_Order')
CREATE INDEX [IX_MenuItems_IsActive_Order] ON [dbo].[MenuItems] ([IsActive], [Order])

-- Índices en tabla Entradas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Entradas_InsumoId')
CREATE INDEX [IX_Entradas_InsumoId] ON [dbo].[Entradas] ([InsumoId])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Entradas_UsuarioId')
CREATE INDEX [IX_Entradas_UsuarioId] ON [dbo].[Entradas] ([UsuarioId])

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Entradas_FechaEntrada')
CREATE INDEX [IX_Entradas_FechaEntrada] ON [dbo].[Entradas] ([FechaEntrada] DESC)

-- Índices en tablas de catálogos para filtros por estado
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Insumos_Estado_Codigo')
CREATE INDEX [IX_Insumos_Estado_Codigo] ON [dbo].[Insumos] ([Estado], [Codigo])

PRINT '=============================================='
PRINT 'SCRIPT COMPLETADO EXITOSAMENTE'
PRINT 'Todas las tablas han sido creadas o ya existían'
PRINT 'Total de tablas: 16'
PRINT '=============================================='