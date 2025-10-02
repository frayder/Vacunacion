-- Script para probar la conexión a la base de datos SIOS
-- Ejecutar este script para verificar que la conexión funciona
-- Esquema: AnulacionFacturas

USE SIOS;
GO

-- Verificar que las tablas existen en el esquema AnulacionFacturas
SELECT
    s.name as SchemaName,
    t.TABLE_NAME,
    t.TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES t
INNER JOIN sys.schemas s ON t.TABLE_SCHEMA = s.name
WHERE t.TABLE_SCHEMA = 'AnulacionFacturas'
  AND t.TABLE_NAME IN ('Pacientes', 'Peticiones', 'Facturas', 'Comentarios', 'DocumentosSoporte')
ORDER BY t.TABLE_NAME;

-- Verificar estructura de las tablas en el esquema AnulacionFacturas
SELECT
    s.name as SchemaName,
    t.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.IS_NULLABLE,
    CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PK' ELSE '' END as KeyType
FROM INFORMATION_SCHEMA.TABLES t
INNER JOIN sys.schemas s ON t.TABLE_SCHEMA = s.name
INNER JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
LEFT JOIN (
    SELECT ku.TABLE_NAME, ku.COLUMN_NAME, tc.TABLE_SCHEMA
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku
    ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME AND c.TABLE_SCHEMA = pk.TABLE_SCHEMA
WHERE t.TABLE_SCHEMA = 'AnulacionFacturas'
  AND t.TABLE_NAME IN ('Pacientes', 'Peticiones', 'Facturas', 'Comentarios', 'DocumentosSoporte')
ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION;

-- Verificar datos de prueba
SELECT 'Pacientes' as Tabla, COUNT(*) as Registros FROM AnulacionFacturas.Pacientes
UNION ALL
SELECT 'Peticiones' as Tabla, COUNT(*) as Registros FROM AnulacionFacturas.Peticiones
UNION ALL
SELECT 'Facturas' as Tabla, COUNT(*) as Registros FROM AnulacionFacturas.Facturas
UNION ALL
SELECT 'Comentarios' as Tabla, COUNT(*) as Registros FROM AnulacionFacturas.Comentarios
UNION ALL
SELECT 'DocumentosSoporte' as Tabla, COUNT(*) as Registros FROM AnulacionFacturas.DocumentosSoporte;

PRINT 'Verificación completada. Si no hay errores, la estructura de la base de datos en el esquema AnulacionFacturas es correcta.';
GO