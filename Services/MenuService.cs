using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Highdmin.Services
{
    public interface IMenuService
    {
        Task<List<MenuViewModel>> GetUserMenusAsync(string userName);
        Task<SidenavViewModel> GetSidenavDataAsync(string userName);
    }

    public class MenuService : IMenuService
    {
        private readonly ApplicationDbContext _context;

        public MenuService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuViewModel>> GetUserMenusAsync(string userName)
        {
            // Obtener el usuario y sus roles
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions!)
                            .ThenInclude(rp => rp.MenuItem)
                .FirstOrDefaultAsync(u => u.UserName == userName); 
            
            if (user == null)
                return new List<MenuViewModel>();

            // Obtener todos los menús con permisos para este usuario
            var userRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();

            var menuItems = await _context.MenuItems
                .Include(m => m.Parent)
                .Include(m => m.Children)
                .Where(m => m.IsActive)
                .OrderBy(m => m.Order)
                .ThenBy(m => m.Name)
                .ToListAsync();

            var rolePermissions = await _context.RolePermissions
                .Include(rp => rp.MenuItem)
                .Include(rp => rp.Role)
                .Where(rp => userRoleIds.Contains(rp.RoleId))
                .ToListAsync();

            // Convertir a ViewModels y aplicar permisos
            var menuViewModels = new List<MenuViewModel>();

            foreach (var menuItem in menuItems.Where(m => m.ParentId == null))
            {
                var menuViewModel = ConvertToViewModel(menuItem, rolePermissions);
                if (menuViewModel != null && (menuViewModel.HasPermission || menuViewModel.Children.Any()))
                {
                    menuViewModels.Add(menuViewModel);
                }
            }

            return menuViewModels.OrderBy(m => m.Order).ToList();
        }

        public async Task<SidenavViewModel> GetSidenavDataAsync(string userName)
        {
            var menus = await GetUserMenusAsync(userName);
            
            var user = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.UserName == userName);

            var userRole = user?.UserRoles.FirstOrDefault()?.Role?.Nombre ?? "Usuario";

            return new SidenavViewModel
            {
                MenuItems = menus,
                UserName = userName ?? "Usuario",
                UserRole = userRole
            };
        }

        private MenuViewModel? ConvertToViewModel(MenuItem menuItem, List<RolePermission> rolePermissions)
        {
            var permissions = rolePermissions.Where(rp => rp.MenuItemId == menuItem.Id).ToList();
            
            var hasAnyPermission = permissions.Any(p => p.CanRead || p.CanCreate || p.CanUpdate || p.CanDelete);

            var viewModel = new MenuViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name ?? string.Empty,
                Resource = menuItem.Resource,
                Icon = menuItem.Icon,
                Url = menuItem.Url,
                Controller = menuItem.Controller,
                Action = menuItem.Action,
                ParentId = menuItem.ParentId,
                Order = menuItem.Order,
                IsActive = menuItem.IsActive,
                HasPermission = hasAnyPermission,
                CanCreate = permissions.Any(p => p.CanCreate),
                CanRead = permissions.Any(p => p.CanRead),
                CanUpdate = permissions.Any(p => p.CanUpdate),
                CanDelete = permissions.Any(p => p.CanDelete)
            };

            // Procesar hijos recursivamente
            foreach (var child in menuItem.Children.Where(c => c.IsActive).OrderBy(c => c.Order))
            {
                var childViewModel = ConvertToViewModel(child, rolePermissions);
                if (childViewModel != null && childViewModel.HasPermission)
                {
                    viewModel.Children.Add(childViewModel);
                }
            }

            return viewModel;
        }
    }
}