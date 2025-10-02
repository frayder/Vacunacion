-- Script para insertar tipos de carnet de vacunación
-- Este script puede ejecutarse independientemente para agregar datos de ejemplo

-- Limpiar datos existentes (opcional)
DELETE FROM TiposCarnet;

-- Insertar Tipos de Carnet
INSERT INTO TiposCarnet (Codigo, Nombre, Descripcion, Estado, FechaCreacion) VALUES
('PAI', 'PAI (Programa Ampliado de Inmunizaciones)', 'Carnet del Programa Ampliado de Inmunizaciones para esquemas regulares de vacunación', 1, GETDATE()),
('COVID19', 'COVID-19', 'Carnet específico para vacunas COVID-19', 1, GETDATE()),
('FIEAMAR', 'Fiebre Amarilla', 'Certificado de vacunación contra Fiebre Amarilla', 1, GETDATE()),
('INTL', 'Certificado Internacional', 'Certificado internacional de vacunación y profilaxis', 1, GETDATE()),
('ESPECIAL', 'Esquema Especial', 'Esquema de vacunación especial por condiciones médicas', 1, GETDATE()),
('VIAJERO', 'Vacunas del Viajero', 'Esquema de vacunación para viajeros internacionales', 1, GETDATE()),
('OCUPAC', 'Vacunación Ocupacional', 'Esquema de vacunación por riesgo ocupacional', 1, GETDATE()),
('OTRO', 'Otro', 'Otros tipos de carnet de vacunación', 1, GETDATE());

PRINT 'Tipos de carnet insertados exitosamente';

-- Verificar la inserción
SELECT COUNT(*) as TotalTiposCarnet FROM TiposCarnet WHERE Estado = 1;
SELECT * FROM TiposCarnet ORDER BY Codigo;