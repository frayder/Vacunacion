using System.Security.Claims;

namespace Highdmin.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmpresaService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentEmpresaId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }

            var empresaIdClaim = httpContext.User.FindFirst("EmpresaId"); 
            if (empresaIdClaim != null && int.TryParse(empresaIdClaim.Value, out int empresaId))
            {
                return empresaId;
            }

            // Debug information - listar todos los claims disponibles
            var allClaims = httpContext.User.Claims.Select(c => $"{c.Type}: {c.Value}").ToList();
            var claimsString = string.Join(", ", allClaims);
            
            throw new UnauthorizedAccessException($"No se pudo determinar la empresa del usuario. Claims disponibles: {claimsString}");
        }

        public string GetCurrentEmpresaNombre()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return "Usuario no autenticado";
            }

            return httpContext.User.FindFirst("EmpresaNombre")?.Value ?? "Empresa no identificada";
        }

        public int GetCurrentUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            
            throw new UnauthorizedAccessException("No se pudo determinar el usuario.");
        }

        public bool IsUserFromEmpresa(int empresaId)
        {
            try
            {
                return GetCurrentEmpresaId() == empresaId;
            }
            catch
            {
                return false;
            }
        }
    }
}