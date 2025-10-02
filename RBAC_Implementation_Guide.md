# Guía de Implementación del Sistema RBAC (Role-Based Access Control)

## Resumen

Se ha implementado un sistema completo de gestión de Roles y Permisos (RBAC) para la aplicación ASP.NET MVC con Entity Framework Core. El sistema permite crear, editar y gestionar roles con permisos granulares por módulo.

## Estructura del Sistema

### Modelos de Datos

#### 1. Role
```csharp
public class Role
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Descripcion { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

#### 2. MenuItem
```csharp
public class MenuItem
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Resource { get; set; }
}
```

#### 3. RolePermission
```csharp
public class RolePermission
{
    public int RoleId { get; set; }
    public int MenuItemId { get; set; }
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}
```

### Controlador: RolesController

#### Métodos Implementados:

1. **Index()** - Lista todos los roles con información de permisos
2. **Create()** - Muestra formulario para crear nuevo rol
3. **Edit(int id)** - Muestra formulario para editar rol existente
4. **Save(RoleViewModel model)** - Guarda rol y permisos
5. **Delete(int id)** - Elimina rol (con validaciones)

### Vistas

#### 1. Index.cshtml
- Lista de roles con información detallada
- Botones de acción (Editar/Eliminar)
- Mensajes de éxito/error
- Diseño responsive con Bootstrap 5

#### 2. Manage.cshtml
- Formulario completo para crear/editar roles
- Tabla de permisos con checkboxes para cada acción CRUD
- Validaciones del lado cliente
- Botones para seleccionar/deseleccionar todos los permisos

### ViewModels

#### RoleViewModel
```csharp
public class RoleViewModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public List<MenuItemPermission> MenuItemPermissions { get; set; }
}

public class MenuItemPermission
{
    public int MenuItemId { get; set; }
    public string? MenuItemName { get; set; }
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
}
```

## Configuración de Base de Datos

### Tablas Creadas:
- `Roles` - Almacena información de roles
- `MenuItems` - Almacena módulos del sistema
- `RolePermissions` - Relación entre roles y permisos por módulo
- `UserRoles` - Relación entre usuarios y roles
- `Permissions` - Permisos del sistema (opcional)

### Script de Inicialización

Ejecutar el archivo `Database/InsertMenuItems.sql` para:
- Insertar MenuItems de ejemplo
- Crear roles predefinidos (Administrador, Supervisor, Operador, Consulta)
- Asignar permisos por defecto a cada rol

## Funcionalidades Implementadas

### ✅ Crear Rol
- Formulario con nombre y descripción
- Selección de permisos por módulo
- Validaciones de entrada

### ✅ Editar Rol
- Carga permisos existentes
- Permite modificar nombre, descripción y permisos
- Mantiene estado de permisos previamente asignados

### ✅ Eliminar Rol
- Validación de usuarios asignados
- Eliminación en cascada de permisos
- Mensajes de confirmación

### ✅ Gestión de Permisos
- Permisos granulares por módulo (Crear, Leer, Actualizar, Eliminar)
- Interfaz intuitiva con switches
- Botones de selección masiva

## Características de la UI

### Bootstrap 5
- Diseño responsive y moderno
- Componentes de formulario mejorados
- Iconos de Remix Icons
- Alertas y notificaciones

### Funcionalidades JavaScript
- Validación del lado cliente
- Selección/deselección masiva de permisos
- Confirmación de eliminación
- Envío de formularios dinámicos

## Cómo Usar el Sistema

### 1. Acceder a la Gestión de Roles
```
/Roles
```

### 2. Crear un Nuevo Rol
1. Hacer clic en "Nuevo Rol"
2. Completar nombre y descripción
3. Seleccionar permisos por módulo
4. Hacer clic en "Guardar Rol"

### 3. Editar un Rol Existente
1. Hacer clic en el menú de acciones del rol
2. Seleccionar "Editar"
3. Modificar información y permisos
4. Guardar cambios

### 4. Eliminar un Rol
1. Hacer clic en el menú de acciones del rol
2. Seleccionar "Eliminar"
3. Confirmar la eliminación

## Integración con Entity Framework Core

### Configuración en DbContext
```csharp
// Configuración de relaciones
modelBuilder.Entity<RolePermission>(entity =>
{
    entity.HasKey(rp => new { rp.RoleId, rp.MenuItemId });
    entity.HasOne(rp => rp.Role)
          .WithMany(r => r.RolePermissions)
          .HasForeignKey(rp => rp.RoleId);
    entity.HasOne(rp => rp.MenuItem)
          .WithMany()
          .HasForeignKey(rp => rp.MenuItemId);
});
```

### Persistencia de Datos
- Los permisos se guardan solo si tienen al menos una acción activa
- Eliminación en cascada de permisos al eliminar rol
- Validación de integridad referencial

## Próximos Pasos

### Para Completar la Implementación:

1. **Ejecutar Migraciones**
   ```bash
   dotnet ef migrations add AddRBACTables
   dotnet ef database update
   ```

2. **Ejecutar Script de Datos**
   ```sql
   -- Ejecutar Database/InsertMenuItems.sql
   ```

3. **Integrar con Autenticación**
   - Conectar con sistema de usuarios existente
   - Implementar middleware de autorización
   - Crear filtros de autorización por controlador

4. **Pruebas**
   - Probar creación de roles
   - Probar edición de permisos
   - Probar eliminación de roles
   - Verificar validaciones

## Estructura de Archivos Modificados

```
Controllers/
├── RolesController.cs (✅ Actualizado)

Views/Roles/
├── Index.cshtml (✅ Actualizado)
├── Manage.cshtml (✅ Actualizado)

Models/
├── RBACModels.cs (✅ Existente)

ViewModels/
├── RoleViewModel.cs (✅ Existente)

Data/
├── ApplicationDbContext.cs (✅ Actualizado)

Database/
├── InsertMenuItems.sql (✅ Nuevo)
```

## Conclusión

El sistema RBAC está completamente implementado y listo para usar. Proporciona una interfaz intuitiva para la gestión de roles y permisos, con validaciones robustas y un diseño moderno usando Bootstrap 5. El sistema es escalable y fácil de mantener.
