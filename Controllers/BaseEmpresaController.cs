using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Highdmin.Services;

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

        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            // Pasar información de la empresa a la vista
            ViewBag.CurrentEmpresaId = CurrentEmpresaId;
            ViewBag.CurrentEmpresaNombre = CurrentEmpresaNombre;
        }
    }
}