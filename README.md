# Sistema de Anulación de Facturas

Sistema web ASP.NET Core MVC para la gestión de solicitudes de anulación de facturas con integración a base de datos SQL Server LocalDB.

## 🚀 Características

- ✅ Gestión completa de peticiones de anulación
- ✅ Interfaz moderna con Bootstrap 5
- ✅ Estadísticas en tiempo real
- ✅ Sistema de aprobación/rechazo
- ✅ Carga de documentos de soporte
- ✅ Base de datos SQL Server LocalDB (Esquema: `AnulacionFacturas`)
- ✅ Sistema de autenticación de usuarios
- ✅ Despliegue automatizado en IIS
- ✅ Operaciones CRUD completas
- ✅ Carga masiva via Excel
- ✅ **Manejo robusto de errores de base de datos**
- ✅ **Mensajes de error amigables al usuario**

## 🛡️ Manejo de Errores

### Errores de Base de Datos
Cuando ocurre un error de conexión a la base de datos, el sistema:

- **Muestra mensajes amigables** en lugar de errores técnicos
- **Redirige automáticamente** a una página de error específica
- **Registra errores internamente** para debugging
- **Permite reintentar** la operación después de un tiempo

### Página de Error de Base de Datos
- URL: `/Error/DatabaseError`
- Mensaje: "No se pudo conectar a la base de datos. Por favor, inténtelo de nuevo en unos momentos."
- Auto-refresh: Se recarga automáticamente después de 30 segundos

### Middleware de Manejo de Errores
- Captura excepciones `SqlException` y `DbUpdateException`
- Redirige a páginas de error apropiadas
- Mantiene la aplicación funcionando incluso con problemas de BD

## 📋 Requisitos

- .NET 9.0 SDK
- SQL Server Express LocalDB
- IIS (Internet Information Services)
- PowerShell 5.1 o superior
- Visual Studio Code (recomendado)
- Node.js y npm (para assets frontend)

## 🛠️ Instalación y Configuración

### Opción 1: Despliegue Completo Automatizado (Recomendado)

```powershell
# Abrir PowerShell como ADMINISTRADOR
cd "D:\INNOVATIONANALYTICS\Anulacion Facturas"

# Ejecutar despliegue completo
.\Deploy-Complete.ps1
```

**Resultado:** La aplicación estará disponible en `http://localhost:8080`

### Opción 2: Configuración Manual

#### 1. Configurar LocalDB

```powershell
# Configurar LocalDB
.\Setup-LocalDB.ps1
```

#### 2. Inicializar Base de Datos

```powershell
# Crear tablas y datos iniciales
.\Initialize-Database.ps1
```

#### 3. Desplegar en IIS

```powershell
# Desplegar aplicación en IIS
.\Deploy-To-IIS.ps1
```

### 3. Verificar Configuración

La cadena de conexión está configurada para LocalDB en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AnulacionFacturas;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

## 🔐 Acceso a la Aplicación

Después del despliegue, acceder a la aplicación con:

- **URL**: `http://localhost:8080`
- **Usuario**: `admin`
- **Contraseña**: `admin123`

## 🏃‍♂️ Ejecutar la Aplicación

### Opción A: Desde IIS (Recomendado)
La aplicación se ejecuta automáticamente en IIS después del despliegue.

### Opción B: Desarrollo Local

```bash
# Navegar al directorio del proyecto
cd "D:\INNOVATIONANALYTICS\Anulacion Facturas"

# Ejecutar la aplicación
dotnet run
```

La aplicación estará disponible en:
- **URL**: `https://localhost:5001` (desarrollo)
- **Dashboard**: `https://localhost:5001/Dashboard`
- **Solicitudes Recibidas**: `https://localhost:5001/Peticiones/Recibidas`

## 📁 Estructura del Proyecto

```
Anulacion Facturas/
├── Controllers/
│   └── PeticionesController.cs      # Controlador principal
├── Models/
│   └── Peticion.cs                  # Modelos de datos
├── Views/
│   ├── Peticiones/
│   │   ├── Index.cshtml            # Lista de peticiones
│   │   ├── Create.cshtml           # Crear nueva petición
│   │   ├── Recibidas.cshtml        # Solicitudes recibidas
│   │   └── Details.cshtml          # Detalles de petición
│   └── Shared/
│       └── _VerticalLayout.cshtml  # Layout principal
├── Data/
│   └── ApplicationDbContext.cs     # Contexto de EF Core
├── Database/
│   ├── CreateTables.sql           # Script de creación de tablas
│   ├── InsertTestData.sql         # Datos de prueba
│   └── TestConnection.sql         # Verificación de BD
├── Setup-Database.ps1             # Script de configuración
└── Program.cs                     # Configuración de la app
```

## 🔧 Funcionalidades Implementadas

### 📊 Dashboard de Solicitudes Recibidas
- Estadísticas en tiempo real (Total, Pendientes, Aprobadas, Rechazadas)
- Tabla responsive con todas las peticiones
- Filtros por estado y prioridad
- Búsqueda y ordenamiento

### ✅ Sistema de Aprobación/Rechazo
- Botones de acción condicionales
- Modal para motivo de rechazo obligatorio
- Generación automática de números de aprobación
- Historial de comentarios

### 📄 Gestión de Documentos
- Carga múltiple de archivos
- Validación de tipos de archivo
- Almacenamiento de nombres de archivos

### 👥 Gestión de Pacientes y Facturas
- Relaciones uno a muchos
- Validación de datos
- Búsqueda por identificación

## 🗄️ Estructura de Base de Datos

**Esquema:** `AnulacionFacturas`

### Tablas Principales

#### `Peticiones`
- `Id` (nvarchar(20)) - Clave primaria
- `Asunto` (nvarchar(500)) - Asunto de la petición
- `Motivo` (nvarchar(1000)) - Motivo detallado
- `FechaCreacion` (datetime2) - Fecha de creación
- `Prioridad` (int) - 0:Baja, 1:Media, 2:Alta
- `Estado` (int) - 0:Pendiente, 1:Aprobada, 2:Rechazada
- `NumeroAprobacion` (nvarchar(50)) - Número de aprobación
- `CreadoPor` (nvarchar(100)) - Usuario creador

#### `Facturas`
- `Id` (int) - Clave primaria
- `Numero` (nvarchar(50)) - Número de factura
- `FechaElaboracion` (datetime2) - Fecha de elaboración
- `Valor` (decimal(18,2)) - Valor de la factura
- `Eps` (nvarchar(100)) - EPS asociada
- `PacienteId` (int) - FK a Pacientes
- `FechaRadicacion` (datetime2) - Fecha de radicación
- `PeticionId` (nvarchar(20)) - FK a Peticiones

#### `Pacientes`
- `Id` (int) - Clave primaria
- `NumeroIdentificacion` (nvarchar(20)) - Número de identificación
- `NombreCompleto` (nvarchar(200)) - Nombre completo
- `TipoIdentificacion` (nvarchar(10)) - Tipo de ID
- `FechaCreacion` (datetime2) - Fecha de creación

#### `Comentarios`
- `Id` (int) - Clave primaria
- `Autor` (nvarchar(100)) - Autor del comentario
- `Texto` (nvarchar(1000)) - Texto del comentario
- `Fecha` (datetime2) - Fecha del comentario
- `PeticionId` (nvarchar(20)) - FK a Peticiones

## 🔐 Seguridad

- Anti-forgery tokens en formularios POST
- Validación de entrada de datos
- Protección contra inyección SQL (EF Core)
- Manejo seguro de conexiones a BD

## � Scripts Disponibles

- `Deploy-Complete.ps1` - Despliegue completo automatizado
- `Setup-LocalDB.ps1` - Configuración de LocalDB
- `Initialize-Database.ps1` - Inicialización de base de datos
- `Deploy-To-IIS.ps1` - Despliegue en IIS
- `Diagnose-SQL-Connection.ps1` - Diagnóstico de conexión SQL
- `Test-LocalDB.cs` - Prueba de conexión a LocalDB

## �🐛 Solución de Problemas

### Error de conexión a LocalDB
```powershell
# Verificar estado de LocalDB
sqllocaldb info mssqllocaldb

# Iniciar LocalDB si está detenido
sqllocaldb start mssqllocaldb

# Crear instancia si no existe
sqllocaldb create mssqllocaldb
```

### Error 500 en IIS
1. Verificar logs de IIS en Visor de Eventos
2. Confirmar permisos de carpeta para `IIS_IUSRS`
3. Verificar configuración del Application Pool
4. Revisar cadena de conexión en `appsettings.json`

### Error de autenticación
1. Verificar que el usuario existe en la base de datos
2. Confirmar que la contraseña es correcta (`admin`/`admin123`)
3. Revisar configuración de cookies en `Program.cs`

### Problemas con el despliegue
```powershell
# Ejecutar diagnóstico de conexión
.\Diagnose-SQL-Connection.ps1

# Verificar configuración de IIS
Get-Website -Name "AnulacionFacturas"
Get-WebAppPoolState -Name "AnulacionFacturasPool"
```

### Error al ejecutar scripts SQL
1. Abrir PowerShell como administrador
2. Verificar permisos de escritura en la BD
3. Ejecutar scripts uno por uno para identificar errores

### Error al compilar
1. Verificar instalación de .NET 9.0
2. Restaurar paquetes: `dotnet restore`
3. Limpiar y reconstruir: `dotnet clean && dotnet build`

## 📞 Soporte

Para soporte técnico o preguntas:
- Revisar logs de aplicación en consola
- Verificar estructura de BD con `TestConnection.sql`
- Comprobar configuración en `appsettings.json`

## 🎯 Próximos Pasos

- [ ] Implementar autenticación de usuarios
- [ ] Agregar roles y permisos
- [ ] Implementar notificaciones por email
- [ ] Agregar reportes y estadísticas avanzadas
- [ ] Implementar API REST para integraciones
- [ ] Agregar tests unitarios e integración# Vacunacion
