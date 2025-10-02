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

        // API endpoints para cargar datos de los dropdowns
        [HttpGet]
        public async Task<IActionResult> GetAseguradoras()
        {
            try
            {
                var aseguradoras = await _context.Aseguradoras
                    .Where(a => a.Estado)
                    .OrderBy(a => a.Nombre)
                    .Select(a => new { value = a.Id, text = a.Nombre })
                    .ToListAsync();

                return Json(aseguradoras);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar aseguradoras: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRegimenesAfiliacion()
        {
            try
            {
                var regimenes = await _context.RegimenesAfiliacion
                    .Where(r => r.Estado)
                    .OrderBy(r => r.Nombre)
                    .Select(r => new { value = r.Id, text = r.Nombre })
                    .ToListAsync();

                return Json(regimenes);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar regímenes de afiliación: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPertenenciasEtnicas()
        {
            try
            {
                var pertenencias = await _context.PertenenciasEtnicas
                    .Where(p => p.Estado)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new { value = p.Id, text = p.Nombre })
                    .ToListAsync();

                return Json(pertenencias);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar pertenencias étnicas: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCentrosAtencion()
        {
            try
            {
                var centros = await _context.CentrosAtencion
                    .Where(c => c.Estado)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new { value = c.Id, text = c.Nombre })
                    .ToListAsync();

                return Json(centros);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar centros de atención: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCondicionesUsuarias()
        {
            try
            {
                var condiciones = await _context.CondicionesUsuarias
                    .Where(c => c.Estado)
                    .OrderBy(c => c.Nombre)
                    .Select(c => new { value = c.Id, text = c.Nombre })
                    .ToListAsync();

                return Json(condiciones);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar condiciones usuarias: " + ex.Message });
            }
        }
    }
}