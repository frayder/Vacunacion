-- Script to add missing columns to Usuarios table
USE SIOS;
GO

-- Add Direccion column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AnulacionFacturas.Usuarios') AND name = 'Direccion')
BEGIN
    ALTER TABLE AnulacionFacturas.Usuarios ADD Direccion NVARCHAR(255) NULL;
END

-- Add Sede column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AnulacionFacturas.Usuarios') AND name = 'Sede')
BEGIN
    ALTER TABLE AnulacionFacturas.Usuarios ADD Sede NVARCHAR(100) NULL;
END

-- Add Identificacion column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('AnulacionFacturas.Usuarios') AND name = 'Identificacion')
BEGIN
    ALTER TABLE AnulacionFacturas.Usuarios ADD Identificacion NVARCHAR(50) NULL;
END

PRINT 'Columns added to Usuarios table successfully.';