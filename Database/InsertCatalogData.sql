-- Datos de prueba para los catálogos del sistema de vacunación
-- Este script inserta datos básicos para las tablas de catálogos

-- Limpiar datos existentes (opcional)
DELETE FROM Aseguradoras;
DELETE FROM RegimenesAfiliacion;
DELETE FROM PertenenciasEtnicas;
DELETE FROM CentrosAtencion;

-- Insertar Aseguradoras
INSERT INTO Aseguradoras (Codigo, Nombre, Descripcion, Estado, FechaCreacion) VALUES
('EPS001', 'SURA EPS', 'Entidad Promotora de Salud SURA', 1, GETDATE()),
('EPS002', 'Nueva EPS', 'Nueva Entidad Promotora de Salud', 1, GETDATE()),
('EPS003', 'Sanitas EPS', 'Entidad Promotora de Salud Sanitas', 1, GETDATE()),
('EPS004', 'Compensar EPS', 'Entidad Promotora de Salud Compensar', 1, GETDATE()),
('EPS005', 'Famisanar EPS', 'Entidad Promotora de Salud Famisanar', 1, GETDATE()),
('EPS006', 'Salud Total EPS', 'Entidad Promotora de Salud Salud Total', 1, GETDATE()),
('EPS007', 'Coomeva EPS', 'Entidad Promotora de Salud Coomeva', 1, GETDATE()),
('EPS008', 'Cruz Blanca EPS', 'Entidad Promotora de Salud Cruz Blanca', 1, GETDATE());

-- Insertar Regímenes de Afiliación
INSERT INTO RegimenesAfiliacion (Codigo, Nombre, Descripcion, Estado, FechaCreacion) VALUES
('REG001', 'Régimen Contributivo', 'Personas con capacidad de pago', 1, GETDATE()),
('REG002', 'Régimen Subsidiado', 'Personas sin capacidad de pago', 1, GETDATE()),
('REG003', 'Régimen Especial', 'Fuerzas militares, policía, etc.', 1, GETDATE()),
('REG004', 'Régimen de Excepción', 'ECOPETROL, universidades públicas, etc.', 1, GETDATE()),
('REG005', 'No Asegurado', 'Población no asegurada', 1, GETDATE());

-- Insertar Pertenencias Étnicas
INSERT INTO PertenenciasEtnicas (Codigo, Nombre, Descripcion, Estado, FechaCreacion) VALUES
('ETN001', 'Indígena', 'Pertenencia étnica indígena', 1, GETDATE()),
('ETN002', 'ROM (Gitano)', 'Pertenencia étnica ROM o Gitana', 1, GETDATE()),
('ETN003', 'Raizal', 'Pertenencia étnica Raizal del archipiélago', 1, GETDATE()),
('ETN004', 'Palenquero', 'Pertenencia étnica Palenquera', 1, GETDATE()),
('ETN005', 'Negro(a) o Afrocolombiano(a)', 'Pertenencia étnica Negra o Afrocolombiana', 1, GETDATE()),
('ETN006', 'Ninguno de los anteriores', 'No pertenece a ningún grupo étnico específico', 1, GETDATE());

-- Insertar Centros de Atención
INSERT INTO CentrosAtencion (Codigo, Nombre, Descripcion, Direccion, Telefono, Estado, FechaCreacion) VALUES
('CA001', 'Hospital Universitario San Ignacio', 'Hospital de alta complejidad', 'Carrera 7 No. 40-62, Bogotá', '3208320000', 1, GETDATE()),
('CA002', 'Clínica del Country', 'Clínica privada de alta complejidad', 'Carrera 16 No. 82-57, Bogotá', '6016442777', 1, GETDATE()),
('CA003', 'Hospital San Juan de Dios', 'Hospital público', 'Carrera 10 No. 1-66, Bogotá', '6013685544', 1, GETDATE()),
('CA004', 'Clínica Medilaser', 'Clínica especializada', 'Calle 95 No. 47-14, Bogotá', '6012570000', 1, GETDATE()),
('CA005', 'Centro de Salud Zona Norte', 'Centro de atención primaria', 'Calle 170 No. 54-78, Bogotá', '6014567890', 1, GETDATE()),
('CA006', 'IPS Universitaria León XIII', 'Institución prestadora de servicios', 'Calle 67 No. 53-108, Medellín', '6044441313', 1, GETDATE()),
('CA007', 'Clínica Las Américas', 'Clínica privada', 'Diagonal 75B No. 2A-80, Medellín', '6034445500', 1, GETDATE()),
('CA008', 'Hospital Universitario del Valle', 'Hospital universitario', 'Calle 5 No. 36-08, Cali', '6025580077', 1, GETDATE());

PRINT 'Datos de catálogos insertados exitosamente';