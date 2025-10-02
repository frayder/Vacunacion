-- Script SQL para insertar datos de prueba
-- Base de datos: SIOS
-- Esquema: AnulacionFacturas

USE SIOS;
GO

-- Insertar pacientes de prueba
INSERT INTO AnulacionFacturas.Pacientes (NumeroIdentificacion, TipoIdentificacion, NombreCompleto) VALUES
('12345678', 'CC', 'María Elena García Rodríguez'),
('87654321', 'CC', 'Juan Carlos Pérez López'),
('11223344', 'CC', 'Ana María González Silva'),
('44332211', 'CC', 'Carlos Alberto Rodríguez Martínez');

-- Insertar peticiones de prueba
INSERT INTO AnulacionFacturas.Peticiones (Id, Asunto, Motivo, FechaCreacion, Prioridad, Estado, NumeroAprobacion, CreadoPor) VALUES
('P-001', 'Anulación de facturas duplicadas - EPS Sanitas', 'Se detectaron facturas duplicadas por error en el sistema de facturación', '2024-01-15 10:30:00', 2, 0, '', 'Juan Pérez'),
('P-002', 'Corrección de valores en facturas - EPS Coomeva', 'Los valores de las facturas no corresponden con los servicios prestados', '2024-01-20 14:15:00', 1, 1, 'AP-20240120-141500', 'María González'),
('P-003', 'Anulación por error administrativo - EPS Nueva EPS', 'Facturas generadas por error administrativo del sistema', '2024-01-25 09:45:00', 2, 2, '', 'Carlos Rodríguez'),
('P-004', 'Ajuste de fechas de radicación - EPS Sura', 'Las fechas de radicación no coinciden con las fechas reales de prestación del servicio', '2024-02-01 16:20:00', 0, 0, '', 'Ana López');

-- Insertar facturas relacionadas con las peticiones
INSERT INTO AnulacionFacturas.Facturas (Numero, FechaElaboracion, Valor, Eps, PacienteId, FechaRadicacion, PeticionId) VALUES
('FAC-2024-001', '2024-01-10 08:00:00', 2500000.00, 'EPS Sanitas', 1, '2024-01-12 10:00:00', 'P-001'),
('FAC-2024-002', '2024-01-10 08:00:00', 2500000.00, 'EPS Sanitas', 1, '2024-01-12 10:00:00', 'P-001'),
('FAC-2024-003', '2024-01-18 09:30:00', 1800000.00, 'EPS Coomeva', 2, '2024-01-19 11:15:00', 'P-002'),
('FAC-2024-004', '2024-01-22 14:20:00', 3200000.00, 'EPS Nueva EPS', 3, '2024-01-23 16:45:00', 'P-003'),
('FAC-2024-005', '2024-01-30 10:10:00', 1500000.00, 'EPS Sura', 4, '2024-02-01 12:30:00', 'P-004');

-- Insertar comentarios de prueba
INSERT INTO AnulacionFacturas.Comentarios (Autor, Texto, Fecha, PeticionId) VALUES
('Juan Pérez', 'Solicitud creada por duplicación de facturas detectada en el sistema', '2024-01-15 10:30:00', 'P-001'),
('María González', 'Solicitud aprobada después de verificar la documentación', '2024-01-21 11:00:00', 'P-002'),
('Carlos Rodríguez', 'Solicitud rechazada por falta de documentación soporte', '2024-01-26 15:30:00', 'P-003'),
('Ana López', 'Solicitud pendiente de revisión por el equipo administrativo', '2024-02-01 16:20:00', 'P-004');

-- Insertar documentos de soporte de prueba
INSERT INTO AnulacionFacturas.DocumentosSoporte (NombreArchivo, PeticionId, FechaSubida) VALUES
('reporte_duplicados.pdf', 'P-001', '2024-01-15 10:30:00'),
('evidencia_sistema.xlsx', 'P-001', '2024-01-15 10:30:00'),
('facturas_coomeva.pdf', 'P-002', '2024-01-20 14:15:00'),
('documentacion_nuevaeps.zip', 'P-003', '2024-01-25 09:45:00'),
('fechas_sura.xlsx', 'P-004', '2024-02-01 16:20:00');

-- Insertar datos de prueba para Usuarios
IF NOT EXISTS (SELECT 1 FROM AnulacionFacturas.Usuarios WHERE NombreUsuario = 'admin')
INSERT INTO AnulacionFacturas.Usuarios (NombreUsuario, Email, PasswordHash, NombreCompleto, Rol, Activo)
VALUES ('admin', 'admin@anulacionfacturas.com', 'AQAAAAEAACcQAAAAEBLjouNqAe6vgUfzKp8Ojxq9X5MqGvTX8rO8wHxGJLjNpQX8Qw==', 'Administrador del Sistema', 'Admin', 1);

IF NOT EXISTS (SELECT 1 FROM AnulacionFacturas.Usuarios WHERE NombreUsuario = 'usuario1')
INSERT INTO AnulacionFacturas.Usuarios (NombreUsuario, Email, PasswordHash, NombreCompleto, Rol, Activo)
VALUES ('usuario1', 'usuario1@empresa.com', 'AQAAAAEAACcQAAAAEBLjouNqAe6vgUfzKp8Ojxq9X5MqGvTX8rO8wHxGJLjNpQX8Qw==', 'Juan Pérez', 'Usuario', 1);

IF NOT EXISTS (SELECT 1 FROM AnulacionFacturas.Usuarios WHERE NombreUsuario = 'usuario2')
INSERT INTO AnulacionFacturas.Usuarios (NombreUsuario, Email, PasswordHash, NombreCompleto, Rol, Activo)
VALUES ('usuario2', 'usuario2@empresa.com', 'AQAAAAEAACcQAAAAEBLjouNqAe6vgUfzKp8Ojxq9X5MqGvTX8rO8wHxGJLjNpQX8Qw==', 'María García', 'Usuario', 1);

PRINT 'Usuarios de prueba insertados exitosamente.';
GO