-- Script para agregar el menú de Pacientes
USE [Highdmin]
GO

-- Insertar menú de Pacientes si no existe
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