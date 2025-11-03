using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHashService _passwordHashService;

        public AuthController(ApplicationDbContext context, IPasswordHashService passwordHashService)
        {
            _context = context;
            _passwordHashService = passwordHashService;
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
                    .Include(u => u.Empresa)
                    .FirstOrDefaultAsync(u => (u.UserName == nombreUsuario || u.Email == nombreUsuario) && u.IsActive);

                if (usuario == null)
                {
                    ViewBag.Error = "Usuario no encontrado o inactivo.";
                    return View();
                }

                // Verificar la contraseña
                if (string.IsNullOrEmpty(usuario.PasswordHash) || string.IsNullOrEmpty(usuario.PasswordSalt))
                {
                    ViewBag.Error = "Usuario sin contraseña configurada. Contacte al administrador.";
                    return View();
                }
                Console.WriteLine("Verifying password for user: " + nombreUsuario);
                Console.WriteLine("PasswordHash: " + usuario.PasswordHash);
                Console.WriteLine("PasswordSalt: " + usuario.PasswordSalt);
                var response = _passwordHashService.VerifyPassword(password, usuario.PasswordHash, usuario.PasswordSalt);
                Console.WriteLine("Password verification result:response " + response);
                if (!response)
                {
                    ViewBag.Error = "Contraseña incorrecta.";
                    return View();
                }

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
                ViewBag.Error = "Error interno del servidor. Inténtelo de nuevo.";
                // Log the exception in production
                // _logger.LogError(ex, "Error during login for user: {Username}", nombreUsuario);
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