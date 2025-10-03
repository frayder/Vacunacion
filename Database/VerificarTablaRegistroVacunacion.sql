-- ===================================================================
-- Script: VerificarTablaRegistroVacunacion.sql
-- Descripción: Verificar si la tabla RegistroVacunacion existe y mostrar su estructura
-- Fecha: 2025-10-02
-- ===================================================================

USE [HighdminDB]
GO

-- Verificar si la tabla existe
IF EXISTS (SELECT * FROM sysobjects WHERE name='RegistroVacunacion' AND xtype='U')
BEGIN
    PRINT '✓ La tabla RegistroVacunacion existe en la base de datos.'
    
    -- Mostrar información de la tabla
    SELECT 
        'RegistroVacunacion' as Tabla,
        COUNT(*) as TotalColumnas
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'RegistroVacunacion'
    
    -- Mostrar las columnas
    PRINT ''
    PRINT 'Columnas de la tabla RegistroVacunacion:'
    PRINT '========================================='
    
    SELECT 
        COLUMN_NAME as Columna,
        DATA_TYPE as TipoDato,
        IS_NULLABLE as PermiteNulos,
        CHARACTER_MAXIMUM_LENGTH as LongitudMaxima,
        COLUMN_DEFAULT as ValorPorDefecto
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'RegistroVacunacion'
    ORDER BY ORDINAL_POSITION
    
    -- Mostrar claves foráneas
    PRINT ''
    PRINT 'Claves foráneas:'
    PRINT '================='
    
    SELECT 
        fk.name AS 'Nombre_FK',
        t1.name AS 'Tabla',
        c1.name AS 'Columna',
        t2.name AS 'Tabla_Referenciada',
        c2.name AS 'Columna_Referenciada'
    FROM sys.foreign_keys fk
    INNER JOIN sys.tables t1 ON fk.parent_object_id = t1.object_id
    INNER JOIN sys.tables t2 ON fk.referenced_object_id = t2.object_id
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.columns c1 ON fkc.parent_column_id = c1.column_id AND fkc.parent_object_id = c1.object_id
    INNER JOIN sys.columns c2 ON fkc.referenced_column_id = c2.column_id AND fkc.referenced_object_id = c2.object_id
    WHERE t1.name = 'RegistroVacunacion'
    
    -- Mostrar índices
    PRINT ''
    PRINT 'Índices:'
    PRINT '========'
    
    SELECT 
        i.name AS 'Nombre_Indice',
        i.type_desc AS 'Tipo',
        i.is_unique AS 'Es_Unico',
        STRING_AGG(c.name, ', ') AS 'Columnas'
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    INNER JOIN sys.tables t ON i.object_id = t.object_id
    WHERE t.name = 'RegistroVacunacion' AND i.type > 0
    GROUP BY i.name, i.type_desc, i.is_unique
    ORDER BY i.name
    
    -- Contar registros existentes
    DECLARE @count INT
    SELECT @count = COUNT(*) FROM RegistroVacunacion
    PRINT ''
    PRINT 'Total de registros en la tabla: ' + CAST(@count AS VARCHAR(10))
    
END
ELSE
BEGIN
    PRINT '✗ La tabla RegistroVacunacion NO existe en la base de datos.'
    PRINT ''
    PRINT 'Para crearla, ejecute el script: CreateRegistroVacunacionTable.sql'
END