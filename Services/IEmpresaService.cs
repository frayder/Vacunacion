using System.Security.Claims;

namespace Highdmin.Services
{
    public interface IEmpresaService
    {
        int GetCurrentEmpresaId();
        string GetCurrentEmpresaNombre();
        int GetCurrentUserId();
        bool IsUserFromEmpresa(int empresaId);
    }
}