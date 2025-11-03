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
            ViewBag.Roles = await _context.Roles
                .Where(r => r.EmpresaId == CurrentEmpresaId)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(new CreateUserViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            Console.WriteLine("=== DEBUGGING CREATE USER WITH VIEWMODEL ===");
            Console.WriteLine("Creating user: " + (model?.UserName ?? "NULL"));
            Console.WriteLine("Password provided: " + (!string.IsNullOrEmpty(model?.Password)));
            Console.WriteLine("Role ID: " + model?.RoleId);
            Console.WriteLine("ModelState.IsValid: " + ModelState.IsValid);
            
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el rol existe y pertenece a la empresa actual
                    var roleExists = await _context.Roles
                        .AnyAsync(r => r.Id == model.RoleId && r.EmpresaId == CurrentEmpresaId);
                    
                    if (!roleExists)
                    {
                        ModelState.AddModelError("RoleId", "El rol seleccionado no es válido.");
                        ViewBag.Roles = await _context.Roles.OrderBy(r => r.Nombre).ToListAsync();
                        return View(model);
                    }

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
                    
                    // Asignar el rol al usuario
                    var userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = model.RoleId,
                        EmpresaId = CurrentEmpresaId
                    };
                    
                    _context.UserRoles.Add(userRole);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine("User and role assigned successfully!");
                    
                    TempData["SuccessMessage"] = "Usuario creado correctamente con el rol asignado.";
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
            
            ViewBag.Roles = await _context.Roles
                .Where(r => r.EmpresaId == CurrentEmpresaId)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
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

            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(m => m.UserId == id && m.EmpresaId == CurrentEmpresaId);
            
            if (user == null)
            {
                return NotFound();
            }

            // Obtener el rol actual del usuario
            var currentUserRole = user.UserRoles.FirstOrDefault();

            var viewModel = new EditUserViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                RoleId = currentUserRole?.RoleId ?? 0,
                PasswordHash = user.PasswordHash,
                PasswordSalt = user.PasswordSalt,
                LastPasswordChange = user.LastPasswordChange,
                EmpresaId = user.EmpresaId,
                IsActive = user.IsActive
            };

            ViewBag.Roles = await _context.Roles
                .Where(r => r.EmpresaId == CurrentEmpresaId)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
                
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar que el rol existe y pertenece a la empresa actual
                    var roleExists = await _context.Roles
                        .AnyAsync(r => r.Id == model.RoleId && r.EmpresaId == CurrentEmpresaId);
                    
                    if (!roleExists)
                    {
                        ModelState.AddModelError("RoleId", "El rol seleccionado no es válido.");
                        ViewBag.Roles = await _context.Roles
                            .Where(r => r.EmpresaId == CurrentEmpresaId)
                            .OrderBy(r => r.Nombre)
                            .ToListAsync();
                        return View(model);
                    }

                    // Obtener el usuario actual de la base de datos
                    var existingUser = await _context.Users
                        .Include(u => u.UserRoles)
                        .FirstOrDefaultAsync(u => u.UserId == id && u.EmpresaId == CurrentEmpresaId);
                    
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    // Actualizar campos básicos
                    existingUser.UserName = model.UserName;
                    existingUser.Email = model.Email;

                    // Si se proporcionó una nueva contraseña, actualizarla
                    if (!string.IsNullOrEmpty(model.NewPassword))
                    {
                        existingUser.PasswordSalt = _passwordHashService.GenerateSalt();
                        existingUser.PasswordHash = _passwordHashService.HashPassword(model.NewPassword, existingUser.PasswordSalt);
                        existingUser.LastPasswordChange = DateTime.UtcNow;
                    }

                    // Actualizar rol del usuario
                    var currentUserRole = existingUser.UserRoles.FirstOrDefault();
                    
                    if (currentUserRole != null)
                    {
                        // Si ya tiene un rol, actualizarlo
                        if (currentUserRole.RoleId != model.RoleId)
                        {
                            currentUserRole.RoleId = model.RoleId;
                            _context.UserRoles.Update(currentUserRole);
                        }
                    }
                    else
                    {
                        // Si no tiene rol, crear uno nuevo
                        var newUserRole = new UserRole
                        {
                            UserId = existingUser.UserId,
                            RoleId = model.RoleId,
                            EmpresaId = CurrentEmpresaId
                        };
                        _context.UserRoles.Add(newUserRole);
                    }

                    _context.Update(existingUser);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(model.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Roles = await _context.Roles
                .Where(r => r.EmpresaId == CurrentEmpresaId)
                .OrderBy(r => r.Nombre)
                .ToListAsync();
            return View(model);
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