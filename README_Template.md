# 🚀 Plantilla Base RBAC - Sistema de Usuarios, Roles y Dashboard

## 📋 **Descripción**

Esta es una plantilla base reutilizable que proporciona un sistema completo de **gestión de usuarios**, **roles y permisos (RBAC)** con **dashboard administrativo**. La plantilla ha sido limpiada y preparada para ser utilizada como base para nuevos proyectos.

## ✨ **Características Principales**

### 🔐 **Sistema RBAC Completo**
- ✅ Gestión de usuarios
- ✅ Gestión de roles
- ✅ Sistema de permisos granular
- ✅ Autenticación y autorización
- ✅ Permisos CRUD básicos
- ✅ Permisos específicos para administración de usuarios

### 📊 **Dashboard Administrativo**
- ✅ Panel de control principal
- ✅ Estadísticas y métricas
- ✅ Interfaz de usuario moderna
- ✅ Componentes UI reutilizables

### 🎨 **Interfaz Completa**
- ✅ Plantilla administrativa responsive
- ✅ Componentes UI (tablas, formularios, gráficos, mapas, etc.)
- ✅ Iconos y elementos visuales
- ✅ Vistas para usuarios y roles

## 🏗️ **Arquitectura**

```
├── 📁 Controllers/
│   ├── 🔑 AuthController.cs       # Autenticación
│   ├── 👥 UsersController.cs      # Gestión de usuarios
│   ├── 🛡️ RolesController.cs       # Gestión de roles
│   ├── 📊 DashboardController.cs  # Panel principal
│   └── 🎨 [UI Controllers]        # Plantillas UI
├── 📁 Models/
│   ├── 🔐 RBACModels.cs           # Modelos RBAC
│   └── ❌ ErrorViewModel.cs       # Manejo de errores
├── 📁 Views/
│   ├── 🏠 Dashboard/              # Vistas del dashboard
│   ├── 👤 Users/                 # Gestión de usuarios
│   ├── 🛡️ Roles/                  # Gestión de roles
│   ├── 🔐 Auth/                   # Autenticación
│   └── 🎨 [UI Views]             # Componentes UI
├── 📁 Database/
│   ├── 🗃️ RBAC_Model.sql          # Estructura base
│   ├── 📋 CreateTables.sql        # Creación de tablas
│   └── 🔧 [Scripts de setup]      # Scripts de configuración
└── 📁 Examples/
    └── 📖 HowToUseRBAC.md         # Documentación de uso
```

## 🚀 **Inicio Rápido**

### **1. Configurar Base de Datos**

```sql
-- Ejecutar en orden:
1. Database/CreateTables.sql
2. Database/RBAC_Model.sql
3. Database/InsertMenuItems.sql
```

### **2. Configurar Conexión**

Actualizar `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YourConnectionStringHere"
  }
}
```

### **3. Ejecutar Aplicación**

```bash
dotnet run
```

## 🔧 **Personalización**

### **Agregar Nuevos Módulos**

1. **Crear modelo de datos** para tu dominio
2. **Agregar controlador** específico
3. **Crear vistas** correspondientes
4. **Extender permisos** en `RBACModels.cs`
5. **Actualizar menús** en base de datos

### **Ejemplo: Agregar Módulo "Productos"**

```csharp
// 1. En RBACModels.cs - Agregar permisos
public bool CanManageProducts { get; set; }

// 2. Crear ProductsController.cs
[RequirePermission("Products", "read")]
public class ProductsController : Controller { }

// 3. Crear Views/Products/
// 4. Agregar menú en base de datos
```

## 📚 **Documentación**

- 📖 **[Guía RBAC Completa](Examples/HowToUseRBAC.md)** - Cómo implementar permisos
- 🔧 **[Scripts de BD](Database/)** - Configuración de base de datos
- 🎨 **[Componentes UI](Views/)** - Elementos de interfaz disponibles

## 🛡️ **Sistema de Permisos**

### **Permisos Base Incluidos:**
- ✅ `CanCreate` - Crear registros
- ✅ `CanRead` - Leer/Ver registros  
- ✅ `CanUpdate` - Actualizar registros
- ✅ `CanDelete` - Eliminar registros
- ✅ `CanActivate` - Activar/Desactivar usuarios
- ✅ `CanResetPassword` - Resetear contraseñas

### **Roles Sugeridos:**
- 🔴 **SuperAdmin** - Acceso completo
- 🟠 **Admin** - Administración limitada
- 🟡 **Manager** - Gestión de usuarios
- 🟢 **User** - Usuario estándar
- 🔵 **Viewer** - Solo lectura

## 🎯 **Casos de Uso**

Esta plantilla es perfecta para:

- ✅ **Sistemas administrativos**
- ✅ **ERPs y CRMs**
- ✅ **Plataformas de gestión**
- ✅ **Aplicaciones empresariales**
- ✅ **Sistemas de control de acceso**

## 🔄 **Migración desde Proyectos Existentes**

Si vienes de un proyecto con datos específicos:

1. **Backup** de datos importantes
2. **Migrar usuarios** existentes
3. **Configurar roles** según tu organización
4. **Personalizar permisos** según necesidades
5. **Agregar módulos** específicos de tu negocio

## 🆘 **Soporte**

Para dudas sobre implementación, consulta:
- 📖 `Examples/HowToUseRBAC.md`
- 🗃️ Scripts en `Database/`
- 🎨 Ejemplos en `Views/`

## 🏷️ **Versión**

**v1.0** - Plantilla Base Limpia
- ✅ RBAC completo
- ✅ Dashboard funcional  
- ✅ UI components
- ✅ Documentación actualizada

---

**¡Listo para usar y personalizar! 🚀**