using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Highdmin.ViewComponents
{
    public class ImportarPlantillaViewComponent : ViewComponent
    {
        /// <summary>
        /// Invoca el componente de importación.
        /// Parámetros: model (el viewmodel), title, downloadAction, indexAction, importAction, saveAction, loadedCollection.
        /// </summary>
        public async Task<IViewComponentResult> InvokeAsync(
            object model,
            string title = "Importar",
            string downloadAction = "DescargarPlantilla",
            string indexAction = "Index",
            string importAction = "ImportarPlantilla",
            string saveAction = "GuardarImportados",
            string? loadedCollection = "",
            string controller = "")
        {
            // Pasar parámetros a la vista del componente mediante ViewData
            ViewData["Title"] = title;
            ViewData["DownloadAction"] = downloadAction;
            ViewData["IndexAction"] = indexAction;
            ViewData["ImportAction"] = importAction;
            ViewData["SaveAction"] = saveAction;

            // nuevo: pasar el nombre del controlador (siempre que se pase)
            ViewData["Controller"] = controller ?? "";

            // Asegurar un valor por defecto razonable para la colección cargada
            ViewData["LoadedCollection"] = string.IsNullOrWhiteSpace(loadedCollection) ? "TiposCarnetCargados" : loadedCollection;

            // No hay trabajo async por ahora, pero dejamos el método async por si se agrega lógica.
            return View(model);
        }
    }
}