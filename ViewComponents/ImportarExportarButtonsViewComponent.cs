using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Highdmin.ViewComponents
{
    public class ImportExportButtonsViewComponent : ViewComponent
    {
        /// <summary>
        /// Parámetros disponibles (todos opcionales):
        /// - controller: nombre del controlador (por defecto: current controller desde ViewContext)
        /// - importAction: nombre de la acción para importar (por defecto "ImportarPlantilla")
        /// - exportAction: nombre de la acción para exportar (por defecto "Exportar")
        /// - importLabel, exportLabel: etiquetas de los botones
        /// - showImport, showExport: booleans para mostrar/ocultar botones
        /// - importClass, exportClass: clases CSS para los botones
        /// </summary>
        public Task<IViewComponentResult> InvokeAsync(
            string? controller = null,
            string importAction = "ImportarPlantilla",
            string exportAction = "Exportar",
            string importLabel = "Importar",
            string exportLabel = "Exportar",
            bool showImport = true,
            bool showExport = true,
            string? importClass = null,
            string? exportClass = null)
        {
            // Si no se pasa controller, intentar inferir el actual (útil para llamadas sencillas)
            if (string.IsNullOrWhiteSpace(controller))
            {
                controller = (string?)ViewContext.RouteData.Values["controller"] ?? "";
            }

            var model = new ImportExportButtonsViewModel
            {
                Controller = controller,
                ImportAction = importAction,
                ExportAction = exportAction,
                ImportLabel = importLabel,
                ExportLabel = exportLabel,
                ShowImport = showImport,
                ShowExport = showExport,
                ImportClass = string.IsNullOrWhiteSpace(importClass) ? "btn btn-info" : importClass,
                ExportClass = string.IsNullOrWhiteSpace(exportClass) ? "btn btn-warning" : exportClass
            };

            return Task.FromResult<IViewComponentResult>(View(model));
        }
    }

    // Pequeño DTO interno para pasar a la vista del componente
    public class ImportExportButtonsViewModel
    {
        public string Controller { get; set; } = "";
        public string ImportAction { get; set; } = "ImportarPlantilla";
        public string ExportAction { get; set; } = "Exportar";
        public string ImportLabel { get; set; } = "Importar";
        public string ExportLabel { get; set; } = "Exportar";
        public bool ShowImport { get; set; } = true;
        public bool ShowExport { get; set; } = true;
        public string ImportClass { get; set; } = "btn btn-info";
        public string ExportClass { get; set; } = "btn btn-warning";
    }
}