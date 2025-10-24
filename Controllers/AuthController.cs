using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Highdmin.Data;
using Highdmin.Models;

namespace Highdmin.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string nombreUsuario, string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Por favor, ingrese usuario y contraseña.";
                return View();
            }

            try
            {
                // Buscar usuario por nombre de usuario o email
                var usuario = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .Include(u => u.Empresa) // <-- Agrega esta línea
                    .FirstOrDefaultAsync(u => u.UserName == nombreUsuario || u.Email == nombreUsuario);

                if (usuario == null)
                {
                    ViewBag.Error = "Usuario no encontrado.";
                    return View();
                }

                // Para la plantilla, aceptamos cualquier contraseña (en producción usar hash real)
                // Crear claims de autenticación  
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.UserId.ToString()),
                    new Claim(ClaimTypes.Name, usuario.UserName ?? ""),
                    new Claim(ClaimTypes.Email, usuario.Email ?? ""),
                    new Claim(ClaimTypes.Role, usuario.UserRoles?.FirstOrDefault()?.Role?.Nombre ?? "User"),
                    new Claim("EmpresaId", usuario.EmpresaId.ToString()),
                    new Claim("EmpresaNombre", usuario.Empresa?.Nombre ?? string.Empty)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = false,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                    new ClaimsPrincipal(claimsIdentity), authProperties);

                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                
                ViewBag.Error = "Error interno del servidor. Inténtelo de nuevo." + ex.Message;
                return View();
            }
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}