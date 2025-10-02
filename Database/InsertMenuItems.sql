-- Script para insertar MenuItems de ejemplo para el sistema RBAC
-- Ejecutar este script después de crear las tablas RBAC

USE [AnulacionFacturas]
GO

-- Insertar MenuItems de ejemplo
INSERT INTO [AnulacionFacturas].[MenuItems] ([Name], [Resource])
VALUES 
    ('Dashboard', 'Dashboard'),
    ('Peticiones', 'Peticiones'),
    ('Facturas', 'Facturas'),
    ('Usuarios', 'Usuarios'),
    ('Roles', 'Roles'),
    ('Reportes', 'Reportes'),
    ('Configuración', 'Configuracion'),
    ('Órdenes', 'Ordenes'),
    ('Pacientes', 'Pacientes'),
    ('Comentarios', 'Comentarios')
GO

-- Insertar algunos roles de ejemplo
INSERT INTO [AnulacionFacturas].[Roles] ([Nombre], [Descripcion])
VALUES 
    ('Administrador', 'Rol con acceso completo al sistema'),
    ('Supervisor', 'Rol con permisos de supervisión y aprobación'),
    ('Operador', 'Rol básico para operaciones diarias'),
    ('Consulta', 'Rol de solo lectura para consultas')
GO

-- Insertar permisos de ejemplo para el rol Administrador
DECLARE @AdminRoleId INT = (SELECT Id FROM [AnulacionFacturas].[Roles] WHERE Nombre = 'Administrador')

INSERT INTO [AnulacionFacturas].[RolePermissions] ([RoleId], [MenuItemId], [CanCreate], [CanRead], [CanUpdate], [CanDelete])
SELECT 
    @AdminRoleId,
    mi.Id,
    1, 1, 1, 1  -- Todos los permisos para administrador
FROM [AnulacionFacturas].[MenuItems] mi
GO

-- Insertar permisos de ejemplo para el rol Supervisor
DECLARE @SupervisorRoleId INT = (SELECT Id FROM [AnulacionFacturas].[Roles] WHERE Nombre = 'Supervisor')

INSERT INTO [AnulacionFacturas].[RolePermissions] ([RoleId], [MenuItemId], [CanCreate], [CanRead], [CanUpdate], [CanDelete])
SELECT 
    @SupervisorRoleId,
    mi.Id,
    CASE 
        WHEN mi.Name IN ('Peticiones', 'Facturas', 'Comentarios') THEN 1
        ELSE 0
    END,
    1,  -- Leer siempre permitido
    CASE 
        WHEN mi.Name IN ('Peticiones', 'Facturas', 'Comentarios') THEN 1
        ELSE 0
    END,
    CASE 
        WHEN mi.Name IN ('Comentarios') THEN 1
        ELSE 0
    END
FROM [AnulacionFacturas].[MenuItems] mi
GO

-- Insertar permisos de ejemplo para el rol Operador
DECLARE @OperadorRoleId INT = (SELECT Id FROM [AnulacionFacturas].[Roles] WHERE Nombre = 'Operador')

INSERT INTO [AnulacionFacturas].[RolePermissions] ([RoleId], [MenuItemId], [CanCreate], [CanRead], [CanUpdate], [CanDelete])
SELECT 
    @OperadorRoleId,
    mi.Id,
    CASE 
        WHEN mi.Name IN ('Peticiones', 'Comentarios') THEN 1
        ELSE 0
    END,
    1,  -- Leer siempre permitido
    CASE 
        WHEN mi.Name IN ('Peticiones', 'Comentarios') THEN 1
        ELSE 0
    END,
    0   -- Eliminar no permitido
FROM [AnulacionFacturas].[MenuItems] mi
GO

-- Insertar permisos de ejemplo para el rol Consulta
DECLARE @ConsultaRoleId INT = (SELECT Id FROM [AnulacionFacturas].[Roles] WHERE Nombre = 'Consulta')

INSERT INTO [AnulacionFacturas].[RolePermissions] ([RoleId], [MenuItemId], [CanCreate], [CanRead], [CanUpdate], [CanDelete])
SELECT 
    @ConsultaRoleId,
    mi.Id,
    0,  -- No crear
    1,  -- Solo leer
    0,  -- No actualizar
    0   -- No eliminar
FROM [AnulacionFacturas].[MenuItems] mi
GO

PRINT 'MenuItems y roles de ejemplo insertados correctamente.'
