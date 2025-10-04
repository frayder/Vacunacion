# Módulo de Base de Datos de Pacientes

Este módulo proporciona una interfaz completa para gestionar la base de datos de pacientes, incluyendo funcionalidades de importación y exportación de datos desde archivos Excel.

## 🎯 Características Principales

- **Vista principal**: Interfaz similar a la imagen de referencia con estadísticas y listado de pacientes
- **Gestión CRUD**: Crear, leer, actualizar y eliminar pacientes
- **Importación Excel**: Cargar datos de pacientes desde archivos Excel
- **Exportación Excel**: Descargar plantilla y datos de pacientes
- **Filtros avanzados**: Búsqueda por nombre, identificación y EPS
- **Historial de cargas**: Seguimiento de importaciones realizadas

## 📋 Estructura de Datos

### Campos del Paciente
- **EPS**: Entidad Promotora de Salud
- **Identificación**: Número de documento único
- **Nombres y Apellidos**: Datos personales del paciente
- **Fecha de Nacimiento**: Para cálculo automático de edad
- **Sexo**: Masculino (M) o Femenino (F)
- **Teléfono**: Número de contacto (opcional)
- **Email**: Correo electrónico (opcional)
- **Dirección**: Dirección de residencia (opcional)
- **Estado**: Activo/Inactivo
- **Fechas de auditoría**: Creación y actualización

## 🚀 Instalación y Configuración

### 1. Ejecutar Script de Base de Datos
Ejecute el script `Database/SetupPacientesModule.sql` en su instancia de SQL Server:

```sql
-- Conectarse a la base de datos Highdmin
USE [Highdmin]
GO

-- Ejecutar el script completo
-- Este script creará:
-- - Tabla Pacientes con índices únicos
-- - Datos de ejemplo
-- - Entrada en el menú de navegación
-- - Permisos básicos para administradores
```

### 2. Verificar Configuración de EPPlus
El módulo requiere EPPlus para el manejo de archivos Excel. La configuración se realiza automáticamente para uso no comercial:

```csharp
// Configuración automática en PacientesController
ExcelPackage.License.Context = OfficeOpenXml.LicenseContext.NonCommercial;
```

### 3. Permisos y Roles
El sistema implementa control de acceso basado en roles (RBAC). Los permisos se configuran automáticamente para el rol Administrador.

## 📊 Interfaz de Usuario

### Vista Principal
La vista principal muestra:
- **Estadísticas**: Total de pacientes, EPS con datos, cargas realizadas
- **Historial de cargas**: Registro de importaciones de archivos
- **Lista de pacientes**: Tabla con filtros y acciones

### Funcionalidades de Importación
1. **Descargar Plantilla**: Obtiene archivo Excel con formato correcto
2. **Cargar Base de Datos**: Importa pacientes desde archivo Excel
3. **Opciones de importación**:
   - Filtrar por EPS específica
   - Sobrescribir datos existentes
   - Validación automática de datos

### Formato de Archivo Excel
Las columnas requeridas son:
- **A**: EPS
- **B**: Identificación  
- **C**: Nombres
- **D**: Apellidos
- **E**: Fecha Nacimiento (YYYY-MM-DD)
- **F**: Sexo (M/F)
- **G**: Teléfono (opcional)
- **H**: Email (opcional)
- **I**: Dirección (opcional)

## 🔧 Configuración Técnica

### Controlador
- **Archivo**: `Controllers/PacientesController.cs`
- **Rutas base**: `/Pacientes/`
- **Autorización**: Requiere autenticación

### Modelos
- **Entidad**: `Models/Paciente.cs`
- **ViewModels**: `ViewModels/PacienteViewModel.cs`

### Vistas
- **Directorio**: `Views/Pacientes/`
- **Vistas disponibles**:
  - `Index.cshtml` - Vista principal
  - `Create.cshtml` - Crear paciente
  - `Edit.cshtml` - Editar paciente
  - `Details.cshtml` - Ver detalles
  - `Delete.cshtml` - Confirmar eliminación
  - `ImportarPlantilla.cshtml` - Importar desde Excel

### Base de Datos
- **Tabla**: `dbo.Pacientes`
- **Índices únicos**: Identificación, Email
- **Claves foráneas**: Ninguna (entidad independiente)

## 🎨 Estilos y Diseño

El módulo utiliza Bootstrap 5 y el tema existente del proyecto, manteniendo consistencia visual con:
- Iconos Remix Icons (`ri-*`)
- Colores del tema principal
- Componentes responsivos
- Animaciones CSS para mejor UX

## 📈 Uso y Ejemplos

### Acceder al Módulo
1. Inicie sesión en la aplicación
2. Navegue al menú "Pacientes" (ícono de usuario)
3. La vista principal mostrará las estadísticas y listado

### Importar Pacientes
1. Haga clic en "Descargar Plantilla"
2. Complete el archivo Excel con los datos
3. Use "Cargar Base de Datos" para importar
4. Configure las opciones según necesidades

### Gestionar Pacientes
- **Crear**: Botón "Nuevo Paciente" o desde listado
- **Editar**: Ícono de lápiz en acciones
- **Ver**: Ícono de ojo para detalles completos
- **Eliminar**: Ícono de papelera con confirmación

## 🔍 Filtros y Búsqueda

La vista principal incluye:
- **Búsqueda global**: Por ID, nombres o apellidos
- **Filtro por EPS**: Dropdown con EPS disponibles
- **Ordenamiento**: Por fecha de registro (más recientes primero)

## 📝 Notas Técnicas

- **Validaciones**: Campos obligatorios y formatos específicos
- **Auditoría**: Fechas de creación y modificación automáticas
- **Seguridad**: Validación de tipos de archivo Excel
- **Performance**: Paginación automática para grandes volúmenes
- **Logs**: Registro de errores y operaciones críticas

## 🚨 Consideraciones de Producción

1. **Backup**: Respaldar base de datos antes de importaciones masivas
2. **Validación**: Verificar integridad de datos en archivos Excel
3. **Rendimiento**: Monitorear consultas con grandes volúmenes
4. **Seguridad**: Configurar permisos de usuario apropiados
5. **Mantenimiento**: Limpiar archivos temporales de importación

## 📞 Soporte

Para reportar problemas o solicitar mejoras:
1. Verificar logs de aplicación en caso de errores
2. Validar permisos de usuario y roles
3. Comprobar conectividad con base de datos
4. Revisar formato de archivos Excel de importación