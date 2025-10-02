-- Script para probar la conexión a la base de datos SIOS
-- Ejecutar este script para verificar que la conexión funciona

USE SIOS;
GO

-- Verificar que las tablas existen
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo'
  AND TABLE_NAME IN ('Pacientes', 'Peticiones', 'Facturas', 'Comentarios', 'DocumentosSoporte');

-- Verificar estructura de las tablas
SELECT
    t.TABLE_NAME,
    c.COLUMN_NAME,
    c.DATA_TYPE,
    c.CHARACTER_MAXIMUM_LENGTH,
    c.IS_NULLABLE,
    CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 'PK' ELSE '' END as KeyType
FROM INFORMATION_SCHEMA.TABLES t
INNER JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
LEFT JOIN (
    SELECT ku.TABLE_NAME, ku.COLUMN_NAME
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS tc
    INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS ku
    ON tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
    AND tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
) pk ON c.TABLE_NAME = pk.TABLE_NAME AND c.COLUMN_NAME = pk.COLUMN_NAME
WHERE t.TABLE_SCHEMA = 'dbo'
  AND t.TABLE_NAME IN ('Pacientes', 'Peticiones', 'Facturas', 'Comentarios', 'DocumentosSoporte')
ORDER BY t.TABLE_NAME, c.ORDINAL_POSITION;

PRINT 'Verificación completada. Si no hay errores, la estructura de la base de datos es correcta.';