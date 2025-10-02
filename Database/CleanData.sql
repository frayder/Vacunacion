-- Script para limpiar datos del esquema AnulacionFacturas
-- Base de datos: SIOS
-- Usar con precaución - elimina todos los datos

USE SIOS;
GO

-- Deshabilitar restricciones de clave foránea temporalmente
ALTER TABLE AnulacionFacturas.Facturas NOCHECK CONSTRAINT FK_Facturas_Pacientes_PacienteId;
ALTER TABLE AnulacionFacturas.Facturas NOCHECK CONSTRAINT FK_Facturas_Peticiones_PeticionId;
ALTER TABLE AnulacionFacturas.Comentarios NOCHECK CONSTRAINT FK_Comentarios_Peticiones_PeticionId;
ALTER TABLE AnulacionFacturas.DocumentosSoporte NOCHECK CONSTRAINT FK_DocumentosSoporte_Peticiones_PeticionId;

-- Limpiar datos en orden inverso de dependencias
DELETE FROM AnulacionFacturas.DocumentosSoporte;
DELETE FROM AnulacionFacturas.Comentarios;
DELETE FROM AnulacionFacturas.Facturas;
DELETE FROM AnulacionFacturas.Peticiones;
DELETE FROM AnulacionFacturas.Pacientes;

-- Reiniciar identity columns
DBCC CHECKIDENT ('AnulacionFacturas.Pacientes', RESEED, 0);
DBCC CHECKIDENT ('AnulacionFacturas.Facturas', RESEED, 0);
DBCC CHECKIDENT ('AnulacionFacturas.Comentarios', RESEED, 0);
DBCC CHECKIDENT ('AnulacionFacturas.DocumentosSoporte', RESEED, 0);

-- Habilitar restricciones de clave foránea nuevamente
ALTER TABLE AnulacionFacturas.Facturas CHECK CONSTRAINT FK_Facturas_Pacientes_PacienteId;
ALTER TABLE AnulacionFacturas.Facturas CHECK CONSTRAINT FK_Facturas_Peticiones_PeticionId;
ALTER TABLE AnulacionFacturas.Comentarios CHECK CONSTRAINT FK_Comentarios_Peticiones_PeticionId;
ALTER TABLE AnulacionFacturas.DocumentosSoporte CHECK CONSTRAINT FK_DocumentosSoporte_Peticiones_PeticionId;

PRINT 'Datos del esquema AnulacionFacturas limpiados exitosamente.';
GO