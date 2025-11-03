using Microsoft.AspNetCore.Mvc;
using Highdmin.Models;
using Highdmin.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Highdmin.Services;
using Highdmin.ViewModels;

namespace Highdmin.Controllers
{
    [Authorize]
    public class UsersController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHashService _passwordHashService;

        public UsersController(
            ApplicationDbContext context, 
            IEmpresaService empresaService, 
            AuthorizationService authorizationService, 
            IPasswordHashService passwordHashService) 
            : base(empresaService, authorizationService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
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
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            Console.WriteLine("=== DEBUGGING CREATE USER WITH VIEWMODEL ===");
            Console.WriteLine("Creating user: " + (model?.UserName ?? "NULL"));
            Console.WriteLine("Password provided: " + (!string.IsNullOrEmpty(model?.Password)));
            Console.WriteLine("ModelState.IsValid: " + ModelState.IsValid);
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Crear el usuario desde el ViewModel
                    var user = new User
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        EmpresaId = CurrentEmpresaId,
                        IsActive = true,
                        PasswordSalt = _passwordHashService.GenerateSalt()
                    };
                    
                    user.PasswordHash = _passwordHashService.HashPassword(model.Password, user.PasswordSalt);
                    user.LastPasswordChange = DateTime.UtcNow;
                    
                    Console.WriteLine("Saving user to database...");
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("User saved successfully!");
                    
                    TempData["SuccessMessage"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear usuario: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                    }
                    ModelState.AddModelError("", "Error al crear el usuario: " + ex.Message);
                }
            }
            
            // Si llegamos aquí, hay errores de validación
            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== ERRORES DE VALIDACIÓN ===");
                foreach (var modelState in ModelState)
                {
                    var key = modelState.Key;
                    var errors = modelState.Value.Errors;
                    if (errors.Count > 0)
                    {
                        Console.WriteLine($"Campo '{key}':");
                        foreach (var error in errors)
                        {
                            Console.WriteLine($"  - {error.ErrorMessage}");
                        }
                    }
                }
                Console.WriteLine("=== FIN ERRORES DE VALIDACIÓN ===");
            }
            
            ViewBag.Roles = await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
            return View(model);
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
        public async Task<IActionResult> Edit(int id, User user, string newPassword)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obtener el usuario actual de la base de datos
                    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id && u.EmpresaId == CurrentEmpresaId);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Actualizar campos básicos
                    existingUser.UserName = user.UserName;
                    existingUser.Email = user.Email;

                    // Si se proporcionó una nueva contraseña, actualizarla
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        existingUser.PasswordSalt = _passwordHashService.GenerateSalt();
                        existingUser.PasswordHash = _passwordHashService.HashPassword(newPassword, existingUser.PasswordSalt);
                        existingUser.LastPasswordChange = DateTime.Now;
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
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