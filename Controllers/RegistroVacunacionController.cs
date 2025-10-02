using Microsoft.AspNetCore.Mvc;
using Highdmin.Data;
using Highdmin.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Highdmin.Controllers
{
    public class RegistroVacunacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegistroVacunacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new RegistroVacunacionViewModel
            {
                TotalRegistros = 0, // Aquí conectarías con la base de datos real
                EsquemasCompletos = 0,
                EsquemasIncompletos = 0,
                Pendientes = 0,
                Registros = new List<RegistroVacunacionItemViewModel>()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Nuevo()
        {
            return View(new RegistroVacunacionItemViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(RegistroVacunacionItemViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                // Aquí implementarías la lógica para guardar el registro
                // Por ahora solo redirigimos
                TempData["Success"] = "Registro de vacunación creado exitosamente";
                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }

        [HttpGet]
        public IActionResult LoadStep(int step)
        {
            // Crear una instancia del modelo vacía o con datos pre-cargados según sea necesario
            var model = new RegistroVacunacionItemViewModel();
            
            // Mapear los números de paso a las vistas parciales correspondientes
            return step switch
            {
                1 => PartialView("Steps/_DatosBasicos", model),
                2 => PartialView("Steps/_DatosComplementarios", model),
                3 => PartialView("Steps/_AntecedentesMedicos", model),
                4 => PartialView("Steps/_CondicionUsuario", model),
                5 => PartialView("Steps/_MedicoCuidador", model),
                6 => PartialView("Steps/_EsquemaVacunacion", model),
                7 => PartialView("Steps/_Responsable", model),
                _ => BadRequest("Paso no válido")
            };
        }

        [HttpPost]
        public async Task<IActionResult> DescargarRegistros()
        {
            // Implementar lógica para generar y descargar archivo Excel/CSV
            // Por ahora retornamos un placeholder
            TempData["Info"] = "Funcionalidad de descarga en desarrollo";
            return RedirectToAction(nameof(Index));
        }
    }
}