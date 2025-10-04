-- =============================================
-- Script para crear la tabla Pacientes
-- Base de datos: Highdmin
-- Fecha: Octubre 2025
-- Descripción: Tabla principal para gestión de pacientes
-- =============================================

USE [Highdmin]
GO

-- Verificar si la tabla ya existe
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND type in (N'U'))
BEGIN
    PRINT 'ADVERTENCIA: La tabla Pacientes ya existe. Se omitirá la creación.'
    PRINT 'Si desea recrearla, elimínela manualmente primero.'
END
ELSE
BEGIN
    PRINT 'Creando tabla Pacientes...'
    
    -- Crear la tabla Pacientes
    CREATE TABLE [dbo].[Pacientes](
        [Id] [int] IDENTITY(1,1) NOT NULL,
        [Eps] [nvarchar](100) NOT NULL,
        [Identificacion] [nvarchar](20) NOT NULL,
        [Nombres] [nvarchar](100) NOT NULL,
        [Apellidos] [nvarchar](100) NOT NULL,
        [FechaNacimiento] [datetime2](7) NOT NULL,
        [Sexo] [nvarchar](10) NOT NULL,
        [Telefono] [nvarchar](15) NULL,
        [Email] [nvarchar](255) NULL,
        [Direccion] [nvarchar](500) NULL,
        [Estado] [bit] NOT NULL CONSTRAINT [DF_Pacientes_Estado] DEFAULT ((1)),
        [FechaCreacion] [datetime2](7) NOT NULL CONSTRAINT [DF_Pacientes_FechaCreacion] DEFAULT (getdate()),
        [FechaActualizacion] [datetime2](7) NULL,
        
        -- Restricción de clave primaria
        CONSTRAINT [PK_Pacientes] PRIMARY KEY CLUSTERED ([Id] ASC)
            WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, 
                  ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
    
    PRINT 'Tabla Pacientes creada exitosamente.'
END
GO

-- Crear índices únicos solo si la tabla fue creada o no existen
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND type in (N'U'))
BEGIN
    -- Índice único para Identificación
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND name = N'IX_Pacientes_Identificacion')
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX [IX_Pacientes_Identificacion] ON [dbo].[Pacientes]
        (
            [Identificacion] ASC
        )
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
              IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, 
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        
        PRINT 'Índice único IX_Pacientes_Identificacion creado.'
    END
    ELSE
    BEGIN
        PRINT 'El índice IX_Pacientes_Identificacion ya existe.'
    END

    -- Índice único para Email (solo para valores no nulos)
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND name = N'IX_Pacientes_Email')
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX [IX_Pacientes_Email] ON [dbo].[Pacientes]
        (
            [Email] ASC
        )
        WHERE ([Email] IS NOT NULL)
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
              IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, 
              ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        
        PRINT 'Índice único IX_Pacientes_Email creado.'
    END
    ELSE
    BEGIN
        PRINT 'El índice IX_Pacientes_Email ya existe.'
    END

    -- Índice no único para búsquedas por EPS
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND name = N'IX_Pacientes_Eps')
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Pacientes_Eps] ON [dbo].[Pacientes]
        (
            [Eps] ASC
        )
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
              DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
              ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        
        PRINT 'Índice IX_Pacientes_Eps creado.'
    END
    ELSE
    BEGIN
        PRINT 'El índice IX_Pacientes_Eps ya existe.'
    END

    -- Índice compuesto para búsquedas por nombre
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND name = N'IX_Pacientes_NombreCompleto')
    BEGIN
        CREATE NONCLUSTERED INDEX [IX_Pacientes_NombreCompleto] ON [dbo].[Pacientes]
        (
            [Nombres] ASC,
            [Apellidos] ASC
        )
        WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, 
              DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, 
              ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        
        PRINT 'Índice IX_Pacientes_NombreCompleto creado.'
    END
    ELSE
    BEGIN
        PRINT 'El índice IX_Pacientes_NombreCompleto ya existe.'
    END
END
GO

-- Insertar datos de ejemplo solo si la tabla está vacía
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND type in (N'U'))
BEGIN
    DECLARE @RowCount INT
    SELECT @RowCount = COUNT(*) FROM [dbo].[Pacientes]
    
    IF @RowCount = 0
    BEGIN
        PRINT 'Insertando datos de ejemplo...'
        
        INSERT INTO [dbo].[Pacientes] 
        ([Eps], [Identificacion], [Nombres], [Apellidos], [FechaNacimiento], [Sexo], [Telefono], [Email], [Direccion], [Estado])
        VALUES 
        ('EPS SURA', '12345678', 'Juan Carlos', 'Pérez López', '1990-01-15', 'M', '3001234567', 'juan.perez@email.com', 'Calle 123 #45-67, Bogotá', 1),
        ('NUEVA EPS', '87654321', 'María José', 'González García', '1985-05-20', 'F', '3007654321', 'maria.gonzalez@email.com', 'Carrera 45 #12-34, Medellín', 1),
        ('SANITAS', '11223344', 'Carlos Alberto', 'Rodríguez Martín', '1992-11-08', 'M', '3009876543', 'carlos.rodriguez@email.com', 'Avenida 80 #23-45, Cali', 1),
        ('COMPENSAR', '55667788', 'Ana María', 'Silva Torres', '1988-03-12', 'F', '3005678901', 'ana.silva@email.com', 'Calle 67 #89-01, Barranquilla', 1),
        ('COOMEVA', '99887766', 'Luis Fernando', 'Jiménez Ruiz', '1995-07-25', 'M', '3003456789', 'luis.jimenez@email.com', 'Carrera 23 #56-78, Bucaramanga', 1),
        ('FAMISANAR', '13579246', 'Diana Carolina', 'Moreno Vásquez', '1993-09-14', 'F', '3002468135', 'diana.moreno@email.com', 'Calle 45 #67-89, Pereira', 1),
        ('SALUD TOTAL', '24681357', 'Andrés Felipe', 'Castro Herrera', '1987-12-03', 'M', '3009753186', 'andres.castro@email.com', 'Avenida 19 #34-56, Manizales', 1),
        ('ALIANSALUD', '98765432', 'Claudia Patricia', 'Ramírez Soto', '1991-04-28', 'F', '3001597534', 'claudia.ramirez@email.com', 'Carrera 15 #78-90, Ibagué', 1)
        
        SELECT @RowCount = @@ROWCOUNT
        PRINT CAST(@RowCount AS VARCHAR(10)) + ' registros de ejemplo insertados.'
    END
    ELSE
    BEGIN
        PRINT 'La tabla ya contiene datos. Se omite la inserción de ejemplos.'
        PRINT 'Total de registros existentes: ' + CAST(@RowCount AS VARCHAR(10))
    END
END
GO

-- Verificar la creación y mostrar estadísticas
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Pacientes]') AND type in (N'U'))
BEGIN
    PRINT '=================================='
    PRINT 'RESUMEN DE LA CREACIÓN'
    PRINT '=================================='
    PRINT 'Tabla: dbo.Pacientes'
    
    -- Contar registros
    DECLARE @TotalRegistros INT
    SELECT @TotalRegistros = COUNT(*) FROM [dbo].[Pacientes]
    PRINT 'Total de registros: ' + CAST(@TotalRegistros AS VARCHAR(10))
    
    -- Listar índices creados
    PRINT ''
    PRINT 'Índices creados:'
    SELECT 
        i.name AS NombreIndice,
        CASE WHEN i.is_unique = 1 THEN 'ÚNICO' ELSE 'NO ÚNICO' END AS Tipo,
        STRING_AGG(c.name, ', ') AS Columnas
    FROM sys.indexes i
    INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE i.object_id = OBJECT_ID('dbo.Pacientes') 
    AND i.name IS NOT NULL
    GROUP BY i.name, i.is_unique
    ORDER BY i.name
    
    PRINT ''
    PRINT '=================================='
    PRINT 'TABLA CREADA EXITOSAMENTE'
    PRINT 'Puede proceder a usar el módulo de Pacientes'
    PRINT '=================================='
END
ELSE
BEGIN
    PRINT 'ERROR: No se pudo crear la tabla Pacientes.'
    PRINT 'Verifique los permisos y la conexión a la base de datos.'
END
GO