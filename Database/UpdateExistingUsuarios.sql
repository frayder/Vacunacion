-- Script to update existing rows in Usuarios table with default values for new columns
USE [YourDatabase];
GO

-- Update existing rows to set default values for new columns
UPDATE dbo.Usuarios
SET Direccion = ISNULL(Direccion, ''),
    Sede = ISNULL(Sede, ''),
    Identificacion = ISNULL(Identificacion, '')
WHERE Direccion IS NULL OR Sede IS NULL OR Identificacion IS NULL;

PRINT 'Existing rows updated with default values for new columns.';