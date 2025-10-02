# Guía de Uso del Sistema RBAC - Plantilla Base

## 🏗️ **Arquitectura Implementada**

### **1. Modelo de Datos RBAC**

```csharp
public class RolePermission
{
    // Permisos básicos CRUD
    public bool CanCreate { get; set; }
    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }

    // Permisos específicos para Usuarios
    public bool CanActivate { get; set; }     // Activar/Desactivar usuarios
    public bool CanResetPassword { get; set; } // Resetear contraseñas
}
```

## 🎯 **Cómo Implementar Controles Personalizados**

### **1. En Controladores (Backend)**

#### **Opción A: Usando Atributos de Autorización**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
[RequirePermission("YourResource", "create")]
public async Task<IActionResult> Create(YourModel model)
{
    // Lógica de creación
    return View();
}

[HttpPost]
[ValidateAntiForgeryToken]
[RequirePermission("YourResource", "update")]
public async Task<IActionResult> Update(int id, YourModel model)
{
    // Lógica de actualización
    return View();
}
```

#### **Opción B: Verificación Manual de Permisos**

```csharp
public class YourController : Controller
{
    private readonly IPermissionService _permissionService;

    public async Task<IActionResult> CustomAction(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        
        if (!await _permissionService.HasPermissionAsync(userId, "YourResource", "customAction"))
        {
            return Forbid("No tiene permisos para realizar esta acción");
        }

        // Lógica de la acción personalizada
        return View();
    }
}
```

### **2. En Vistas (Frontend)**

#### **Mostrar/Ocultar Botones Según Permisos**

```html
@if (await ViewContext.HttpContext.RequestServices.HasPermission("YourResource", "create"))
{
    <button type="submit" class="btn btn-primary">
        <i class="ri-add-line"></i> Crear
    </button>
}

@if (await ViewContext.HttpContext.RequestServices.HasPermission("YourResource", "delete"))
{
    <button type="button" class="btn btn-danger" data-bs-toggle="modal" data-bs-target="#deleteModal">
        <i class="ri-delete-bin-line"></i> Eliminar
    </button>
}
```

#### **Verificar Permisos en JavaScript**

```javascript
// Obtener permisos del usuario desde el servidor
var userPermissions = @Html.Raw(Json.Serialize(ViewBag.UserPermissions));

function canPerformAction(action) {
    return userPermissions.includes(action);
}

// Usar en eventos
document.getElementById('actionBtn').addEventListener('click', function() {
    if (!canPerformAction('customAction')) {
        alert('No tiene permisos para realizar esta acción');
        return;
    }
    // Lógica de la acción personalizada
});
```

## 🔧 **Ejemplos de Implementación**

### **Ejemplo 1: Control de Activación de Usuarios**

```csharp
// En UsersController.cs
[HttpPost]
[RequirePermission("Users", "activate")]
public async Task<IActionResult> ToggleUserStatus(int id)
{
    var usuario = await _context.Users.FindAsync(id);
    if (usuario == null) return NotFound();

    usuario.IsActive = !usuario.IsActive;
    usuario.LastModified = DateTime.Now;

    await _context.SaveChangesAsync();
    return RedirectToAction("Index");
}
```

```html
<!-- En la vista de usuarios -->
@if (await ViewContext.HttpContext.RequestServices.HasPermission("Users", "activate"))
{
    <button type="button" class="btn btn-warning" onclick="toggleUserStatus(@user.Id)">
        <i class="ri-user-settings-line"></i> @(user.IsActive ? "Desactivar" : "Activar")
    </button>
}
```

### **Ejemplo 2: Control de Reseteo de Contraseñas**

```csharp
// En UsersController.cs
[HttpPost]
[RequirePermission("Users", "resetPassword")]
public async Task<IActionResult> ResetPassword(int id)
{
    var usuario = await _context.Users.FindAsync(id);
    if (usuario == null) return NotFound();

    // Lógica de reseteo de contraseña
    var newPassword = GenerateRandomPassword();
    usuario.Password = HashPassword(newPassword);
    
    await _context.SaveChangesAsync();
    
    // Enviar nueva contraseña por email
    await _emailService.SendPasswordResetEmail(usuario.Email, newPassword);
    
    return Json(new { success = true, message = "Contraseña reseteada exitosamente" });
}
```

## 📚 **Patrones de Uso Recomendados**

### **1. Configuración de Roles Estándar**

```sql
-- Roles base recomendados
INSERT INTO Roles (Nombre, Descripcion) VALUES
('SuperAdmin', 'Acceso completo al sistema'),
('Admin', 'Administrador con permisos limitados'),
('User', 'Usuario estándar'),
('Viewer', 'Solo lectura');
```

### **2. Estructura de Permisos Recomendada**

```csharp
public static class PermissionConstants
{
    public const string USERS_CREATE = "users.create";
    public const string USERS_READ = "users.read";
    public const string USERS_UPDATE = "users.update";
    public const string USERS_DELETE = "users.delete";
    public const string USERS_ACTIVATE = "users.activate";
    public const string USERS_RESET_PASSWORD = "users.resetPassword";
    
    public const string ROLES_CREATE = "roles.create";
    public const string ROLES_READ = "roles.read";
    public const string ROLES_UPDATE = "roles.update";
    public const string ROLES_DELETE = "roles.delete";
}
```

### **3. Extensión para Nuevos Módulos**

Para agregar un nuevo módulo con RBAC:

1. **Agregar permisos al modelo RolePermission**
2. **Crear constantes de permisos**
3. **Implementar verificaciones en controladores**
4. **Agregar controles en vistas**
5. **Actualizar base de datos con nuevos menús/permisos**

## 🚀 **Siguientes Pasos**

1. **Personalizar** los permisos según las necesidades de tu aplicación
2. **Agregar** nuevos recursos y acciones específicas
3. **Implementar** la lógica de negocio en los controladores
4. **Diseñar** las vistas con los controles de permisos apropiados
5. **Probar** el sistema con diferentes roles y usuarios

---

**Nota:** Esta es una plantilla base. Personaliza los recursos, acciones y permisos según las necesidades específicas de tu aplicación.