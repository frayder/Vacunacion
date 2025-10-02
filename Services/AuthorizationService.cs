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

            var hasPermission = await _context.RolePermissions
                .Where(rp => userRoles.Contains(rp.RoleId))
                .AnyAsync(rp => _context.Permissions
                    .Any(p => p.PermissionId == rp.PermissionId && p.Resource == resource && p.Action == action));

            return hasPermission;
        }
    }
}