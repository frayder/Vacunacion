using OfficeOpenXml;

namespace Highdmin.Services
{
    public static class EPPlusConfiguration
    {
        public static void Initialize()
        {
            // Para EPPlus 8+, configurar licencia no comercial
            // Esto se hace directamente en el código sin una propiedad estática
            // La licencia se establece mediante el contexto en el momento de uso
        }
    }
}