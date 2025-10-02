using System.Collections.Generic;

namespace Highdmin.ViewModels
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<MenuItemPermission> MenuItemPermissions { get; set; } = new List<MenuItemPermission>();
    }

    public class MenuItemPermission
    {
        public int MenuItemId { get; set; }
        public string? MenuItemName { get; set; }
        
        // Permisos básicos CRUD
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }

        // Permisos específicos para Peticiones
        public bool CanApprove { get; set; }
        public bool CanReject { get; set; }
        public bool CanAssign { get; set; }
        public bool CanComment { get; set; }

        // Permisos específicos para Facturas
        public bool CanAnnull { get; set; }
        public bool CanProcess { get; set; }

        // Permisos específicos para Usuarios
        public bool CanActivate { get; set; }
        public bool CanResetPassword { get; set; }
    }
}