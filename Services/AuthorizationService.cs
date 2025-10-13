using System;
using System.Linq;
using System.Threading.Tasks;
using Highdmin.Data;
using Microsoft.EntityFrameworkCore;

namespace Highdmin.Services
{
    public class AuthorizationService
    {
        private readonly ApplicationDbContext _context;

        public AuthorizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasPermissionAsync(int userId, string resource, string action)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!userRoles.Any()) return false;

            // Buscar por MenuItem.Resource que coincida con el resource solicitado
            var hasPermission = await _context.RolePermissions
                .Include(rp => rp.MenuItem)
                .Where(rp => userRoles.Contains(rp.RoleId) && 
                            rp.MenuItem.Resource == resource)
                .AnyAsync(rp => 
                    (action == "Create" && rp.CanCreate) ||
                    (action == "Read" && rp.CanRead) ||
                    (action == "Update" && rp.CanUpdate) ||
                    (action == "Delete" && rp.CanDelete));

            return hasPermission;
        }

        public async Task<Dictionary<string, bool>> GetUserPermissionsAsync(int userId, string resource)
        {
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            if (!userRoles.Any())
            {
                return new Dictionary<string, bool>
                {
                    { "Create", false },
                    { "Read", false },
                    { "Update", false },
                    { "Delete", false }
                };
            }

            var permissions = await _context.RolePermissions
                .Include(rp => rp.MenuItem)
                .Where(rp => userRoles.Contains(rp.RoleId) && 
                            rp.MenuItem.Resource == resource)
                .ToListAsync();

            return new Dictionary<string, bool>
            {
                { "Create", permissions.Any(p => p.CanCreate) },
                { "Read", permissions.Any(p => p.CanRead) },
                { "Update", permissions.Any(p => p.CanUpdate) },
                { "Delete", permissions.Any(p => p.CanDelete) }
            };
        }
    }
}