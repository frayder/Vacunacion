using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Highdmin.Services;
using System.Security.Claims;

namespace Highdmin.Controllers
{
    [Authorize]
    public abstract class BaseEmpresaController : Controller
    {
        protected readonly IEmpresaService _empresaService;

        protected BaseEmpresaController(IEmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        protected int CurrentEmpresaId => _empresaService.GetCurrentEmpresaId();
        protected string CurrentEmpresaNombre => _empresaService.GetCurrentEmpresaNombre();
        protected int CurrentUserId => _empresaService.GetCurrentUserId();

        // Método para obtener el ID del usuario actual
        protected int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

        // Método para validar permisos específicos
        protected async Task<bool> HasPermissionAsync(AuthorizationService authorizationService, string module, string action)
        {
            var userId = GetCurrentUserId();
            return await authorizationService.HasPermissionAsync(userId, module, action);
        }

        // Método para validar permisos y redirigir automáticamente si no los tiene
        protected async Task<IActionResult?> ValidatePermissionAsync(AuthorizationService authorizationService, string module, string action, string? customErrorMessage = null)
        {
            var hasPermission = await HasPermissionAsync(authorizationService, module, action);
            
            if (!hasPermission)
            {
                var errorMessage = customErrorMessage ?? $"No tiene permisos para {GetActionDescription(action)} {module.ToLower()}.";
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Index", "Dashboard");
            }

            return null; // No hay redirección, el usuario tiene permisos
        }

        // Método para obtener permisos completos del usuario para un módulo
        protected async Task<Dictionary<string, bool>> GetModulePermissionsAsync(AuthorizationService authorizationService, string module)
        {
            var userId = GetCurrentUserId();
            return await authorizationService.GetUserPermissionsAsync(userId, module);
        }

        // Método helper para describir las acciones
        private string GetActionDescription(string action)
        {
            return action switch
            {
                "Read" => "ver",
                "Create" => "crear",
                "Update" => "actualizar",
                "Delete" => "eliminar",
                _ => "acceder a"
            };
        }

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            // Pasar información de la empresa a la vista
            ViewBag.CurrentEmpresaId = CurrentEmpresaId;
            ViewBag.CurrentEmpresaNombre = CurrentEmpresaNombre;
        }
    }
}