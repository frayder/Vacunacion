using Microsoft.AspNetCore.Mvc;
using Highdmin.Services;
using System.Security.Claims;

namespace Highdmin.Controllers
{
    public abstract class BaseAuthorizationController : BaseEmpresaController
    {
        protected readonly AuthorizationService _authorizationService;

        protected BaseAuthorizationController(IEmpresaService empresaService, AuthorizationService authorizationService) 
            : base(empresaService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// Valida si el usuario tiene permisos para una acción específica en un módulo
        /// </summary>
        /// <param name="module">Nombre del módulo</param>
        /// <param name="action">Acción a validar (Read, Create, Update, Delete)</param>
        /// <param name="customErrorMessage">Mensaje de error personalizado</param>
        /// <returns>Null si tiene permisos, RedirectResult si no los tiene</returns>
        protected async Task<IActionResult?> ValidatePermissionAsync(string module, string action, string? customErrorMessage = null)
        {
            var userId = GetCurrentUserId();
            var hasPermission = await _authorizationService.HasPermissionAsync(userId, module, action);
            
            if (!hasPermission)
            {
                var errorMessage = customErrorMessage ?? $"No tiene permisos para {GetActionDescription(action)} {GetModuleDescription(module)}.";
                TempData["ErrorMessage"] = errorMessage;
                return RedirectToAction("Index", "Dashboard");
            }

            return null;
        }

        /// <summary>
        /// Obtiene todos los permisos del usuario para un módulo específico
        /// </summary>
        protected async Task<Dictionary<string, bool>> GetModulePermissionsAsync(string module)
        {
            var userId = GetCurrentUserId();
            return await _authorizationService.GetUserPermissionsAsync(userId, module);
        }

        /// <summary>
        /// Valida permisos y obtiene los permisos del módulo en una sola operación
        /// </summary>
        protected async Task<(IActionResult? redirect, Dictionary<string, bool> permissions)> ValidateAndGetPermissionsAsync(
            string module, 
            string requiredAction = "Read", 
            string? customErrorMessage = null)
        {
            var redirect = await ValidatePermissionAsync(module, requiredAction, customErrorMessage);
            
            if (redirect != null)
            {
                return (redirect, new Dictionary<string, bool>());
            }

            var permissions = await GetModulePermissionsAsync(module);
            return (null, permissions);
        }

        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        }

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

        private string GetModuleDescription(string module)
        {
            return module switch
            {
                "CondicionUsuaria" => "las condiciones usuarias",
                "TipoCarnet" => "los tipos de carnet",
                "Aseguradoras" => "las aseguradoras",
                "RegimenAfiliacion" => "los regímenes de afiliación",
                "PertenenciaEtnica" => "las pertenencias étnicas",
                "Pacientes" => "los pacientes",
                "RegistroVacunacion" => "los registros de vacunación",
                _ => module.ToLower()
            };
        }
    }
}