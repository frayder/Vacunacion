using Microsoft.AspNetCore.Mvc;
using Highdmin.Models;
using Highdmin.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    [Authorize]
    public class UsersController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService, authorizationService)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Validar permisos y obtener todos los permisos del módulo
            var (redirect, permissions) = await ValidateAndGetPermissionsAsync("CondicionUsuaria", "Read");
            if (redirect != null) return redirect;

            var users = await _context.Users
                .Where(u => u.EmpresaId == CurrentEmpresaId)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .OrderBy(u => u.UserName)
                .ToListAsync();

            ViewBag.CanCreate = permissions["Create"];
            ViewBag.CanUpdate = permissions["Update"];
            ViewBag.CanDelete = permissions["Delete"];
            return View(users);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.EmpresaId = CurrentEmpresaId;
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
            return View(user);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(m => m.UserId == id && m.EmpresaId == CurrentEmpresaId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id && m.EmpresaId == CurrentEmpresaId);
            if (user == null)
            {
                return NotFound();
            }
            ViewBag.Roles = await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(m => m.UserId == id && m.EmpresaId == CurrentEmpresaId);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id && m.EmpresaId == CurrentEmpresaId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id && e.EmpresaId == CurrentEmpresaId);
        }
    }
}