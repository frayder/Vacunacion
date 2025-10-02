using Highdmin.Models;

namespace Highdmin.ViewModels
{
    public class MenuViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Resource { get; set; }
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public int? ParentId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
        public bool HasPermission { get; set; }
        public List<MenuViewModel> Children { get; set; } = new List<MenuViewModel>();

        // Propiedades para permisos
        public bool CanCreate { get; set; }
        public bool CanRead { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
    }

    public class SidenavViewModel
    {
        public List<MenuViewModel> MenuItems { get; set; } = new List<MenuViewModel>();
        public string UserName { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
    }
}