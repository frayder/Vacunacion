using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    [Authorize]
    public class RolesController : BaseEmpresaController
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context, IEmpresaService empresaService) : base(empresaService)
        {
            _context = context;
        }

        // GET: /Roles
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.MenuItem)
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return View(roles);
        }

        // GET: /Roles/Create
        public async Task<IActionResult> Create()
        {
            var menuItems = await _context.MenuItems.OrderBy(m => m.Name).ToListAsync();

            var viewModel = new RoleViewModel
            {
                MenuItemPermissions = menuItems.Select(mi => new MenuItemPermission
                {
                    MenuItemId = mi.Id,
                    MenuItemName = mi.Name,
                    CanCreate = false,
                    CanRead = false,
                    CanUpdate = false,
                    CanDelete = false
                }).ToList()
            };

            return View("Manage", viewModel);
        }

        // GET: /Roles/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.MenuItem)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            var menuItems = await _context.MenuItems.OrderBy(m => m.Name).ToListAsync();

            var viewModel = new RoleViewModel
            {
                Id = role.Id,
                Name = role.Nombre,
                Description = role.Descripcion,
                MenuItemPermissions = menuItems.Select(mi =>
                {
                    var existingPermission = role.RolePermissions.FirstOrDefault(rp => rp.MenuItemId == mi.Id);
                    return new MenuItemPermission
                    {
                        MenuItemId = mi.Id,
                        MenuItemName = mi.Name,
                        CanCreate = existingPermission?.CanCreate ?? false,
                        CanRead = existingPermission?.CanRead ?? false,
                        CanUpdate = existingPermission?.CanUpdate ?? false,
                        CanDelete = existingPermission?.CanDelete ?? false
                    };
                }).ToList()
            };

            return View("Manage", viewModel);
        }

        // POST: /Roles/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(RoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var menuItems = await _context.MenuItems.OrderBy(m => m.Name).ToListAsync();
                model.MenuItemPermissions = model.MenuItemPermissions ?? menuItems.Select(mi => new MenuItemPermission
                {
                    MenuItemId = mi.Id,
                    MenuItemName = mi.Name,
                    CanCreate = false,
                    CanRead = false,
                    CanUpdate = false,
                    CanDelete = false
                }).ToList();

                return View("Manage", model);
            }

            try
            {
                Role role;
                bool isNewRole = model.Id == 0;

                if (isNewRole)
                {
                    // Crear nuevo rol
                    role = new Role
                    {
                        Nombre = model.Name,
                        Descripcion = model.Description
                    };
                    _context.Roles.Add(role);
                }
                else
                {
                    // Editar rol existente
                    role = await _context.Roles
                        .Include(r => r.RolePermissions)
                        .FirstOrDefaultAsync(r => r.Id == model.Id);

                    if (role == null)
                    {
                        TempData["ErrorMessage"] = "Rol no encontrado.";
                        return RedirectToAction(nameof(Index));
                    }

                    role.Nombre = model.Name;
                    role.Descripcion = model.Description;

                    // Eliminar permisos existentes
                    _context.RolePermissions.RemoveRange(role.RolePermissions);
                }

                await _context.SaveChangesAsync();

                // Agregar nuevos permisos
                if (model.MenuItemPermissions != null)
                {
                    var rolePermissions = model.MenuItemPermissions
                        .Where(mp => mp.CanCreate || mp.CanRead || mp.CanUpdate || mp.CanDelete)
                        .Select(mp => new RolePermission
                        {
                            RoleId = role.Id,
                            MenuItemId = mp.MenuItemId,
                            CanCreate = mp.CanCreate,
                            CanRead = mp.CanRead,
                            CanUpdate = mp.CanUpdate,
                            CanDelete = mp.CanDelete,
                            EmpresaId = CurrentEmpresaId
                        }).ToList();

                    _context.RolePermissions.AddRange(rolePermissions);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = isNewRole ? "Rol creado exitosamente." : "Rol actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al guardar el rol: {ex.Message}";

                var menuItems = await _context.MenuItems.OrderBy(m => m.Name).ToListAsync();
                model.MenuItemPermissions = model.MenuItemPermissions ?? menuItems.Select(mi => new MenuItemPermission
                {
                    MenuItemId = mi.Id,
                    MenuItemName = mi.Name,
                    CanCreate = false,
                    CanRead = false,
                    CanUpdate = false,
                    CanDelete = false
                }).ToList();

                return View("Manage", model);
            }
        }

        // GET: /Roles/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var role = await _context.Roles
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        // POST: /Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var role = await _context.Roles
                    .Include(r => r.UserRoles)
                    .Include(r => r.RolePermissions)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (role == null)
                {
                    TempData["ErrorMessage"] = "Rol no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Verificar si hay usuarios asignados a este rol
                if (role.UserRoles.Any())
                {
                    TempData["ErrorMessage"] = $"No se puede eliminar el rol '{role.Nombre}' porque tiene usuarios asignados.";
                    return RedirectToAction(nameof(Index));
                }

                // Eliminar permisos del rol
                _context.RolePermissions.RemoveRange(role.RolePermissions);

                // Eliminar el rol
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Rol '{role.Nombre}' eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar el rol: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Roles/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.MenuItem)
                .Include(r => r.UserRoles)
                .ThenInclude(ur => ur.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                TempData["ErrorMessage"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }

            return View(role);
        }

        private bool RoleExists(int id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}