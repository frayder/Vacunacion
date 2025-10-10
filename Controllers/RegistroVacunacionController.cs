using Microsoft.AspNetCore.Mvc;
using Highdmin.Data;
using Highdmin.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Highdmin.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
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
            // Consultamos todos los registros de vacunación desde la base de datos
            var registros = await _context.RegistrosVacunacion
                .Select(r => new RegistroVacunacionItemViewModel
                {
                    Id = r.Id,
                    TipoDocumento = r.TipoDocumento,
                    NumeroDocumento = r.NumeroDocumento,
                    NombreCompleto = r.PrimerNombre + " " + r.SegundoNombre + " " + r.PrimerApellido + " " + r.SegundoApellido,
                    FechaNacimiento = r.FechaNacimiento,
                    // FechaAplicacion = r.FechaAplicacion,
                })
                .ToListAsync();

            // Calculamos estadísticas generales
            var viewModel = new RegistroVacunacionViewModel
            {
                TotalRegistros = registros.Count,
                EsquemasCompletos = 0,
                EsquemasIncompletos = 0,
                // Pendientes = registros.Count(r => r.FechaAplicacion == null),
                Registros = registros
            };

            return View(viewModel);
        }


        [HttpGet]
        public IActionResult Nuevo()
        {
            return View(new RegistroVacunacionItemViewModel());
        }

        // Método auxiliar para generar consecutivo
        private async Task<string> GenerarConsecutivoAsync()
        {
            var año = DateTime.Now.Year;
            var sql = $@"
        BEGIN TRANSACTION;
        DECLARE @Ultimo NVARCHAR(50);
        SELECT TOP 1 @Ultimo = Consecutivo 
        FROM RegistrosVacunacion WITH (UPDLOCK, HOLDLOCK)
        WHERE Consecutivo LIKE 'VAC-{año}-%'
        ORDER BY Consecutivo DESC;

        DECLARE @Nuevo NVARCHAR(50);
        IF @Ultimo IS NULL
            SET @Nuevo = 'VAC-{año}-000001';
        ELSE
            SET @Nuevo = 'VAC-{año}-' + RIGHT('000000' + CAST(CAST(RIGHT(@Ultimo, 6) AS INT) + 1 AS NVARCHAR(6)), 6);

        COMMIT TRANSACTION;
        SELECT @Nuevo;
    ";

            await using var command = _context.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            await _context.Database.OpenConnectionAsync();
            var result = await command.ExecuteScalarAsync();
            await _context.Database.CloseConnectionAsync();

            return result?.ToString() ?? $"VAC-{año}-000001";
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

        [HttpGet]
        public async Task<IActionResult> GetTiposCarnet()
        {
            try
            {
                var tiposCarnet = await _context.TiposCarnet
                    .Where(t => t.Estado)
                    .OrderBy(t => t.Nombre)
                    .Select(t => new { value = t.Id, text = t.Nombre, codigo = t.Codigo })
                    .ToListAsync();

                return Json(tiposCarnet);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar tipos de carnet: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Nuevo(RegistroVacunacionItemViewModel modelo)
        {
            // Lógica temporal - solo mostrar que se recibieron los datos
            if (ModelState.IsValid)
            {
                TempData["Success"] = "Registro recibido correctamente - implementar lógica de guardado";


                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarRegistroCompleto([FromBody] Dictionary<string, object> datos)
        {
            try
            { 
                // Log de los datos recibidos del request ANTES del model binding

                // Crear un modelo para capturar todos los datos del formulario
                var json = System.Text.Json.JsonSerializer.Serialize(datos); 
                // Configuramos las opciones para que acepte números entre comillas
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };
        
                var modelo = JsonSerializer.Deserialize<RegistroVacunacionItemViewModel>(json, options);
 

                // Mapear todos los datos del ViewModel a la entidad del modelo de datos
                var Consecutivo = await GenerarConsecutivoAsync(); 
                var entidad = new RegistrosVacunacion
                {
                    // DATOS BÁSICOS (Paso 1)
                    Consecutivo = Consecutivo,
                    PrimerNombre = modelo.PrimerNombre,
                    SegundoNombre = modelo.SegundoNombre,
                    PrimerApellido = modelo.PrimerApellido,
                    SegundoApellido = modelo.SegundoApellido,
                    TipoDocumento = modelo.TipoDocumento,
                    NumeroDocumento = modelo.NumeroDocumento,
                    FechaNacimiento = modelo.FechaNacimiento ?? DateTime.Now,
                    Genero = modelo.Genero,
                    Telefono = modelo.Telefono,
                    Direccion = modelo.Direccion,
                    NombreCompleto = $"{modelo.PrimerNombre} {modelo.SegundoNombre} {modelo.PrimerApellido} {modelo.SegundoApellido}".Trim(),
                    // DATOS COMPLEMENTARIOS (Paso 2)
                    AseguradoraId = modelo.AseguradoraId,
                    RegimenAfiliacionId = modelo.RegimenAfiliacionId,
                    PertenenciaEtnicaId = modelo.PertenenciaEtnicaId,
                    CentroAtencionId = modelo.CentroAtencionId,
                    Sexo = modelo.Sexo,
                    OrientacionSexual = modelo.OrientacionSexual,
                    EdadGestacional = modelo.EdadGestacional,
                    PesoInfante = modelo.PesoInfante ?? 0,
                    PaisNacimiento = modelo.PaisNacimiento,
                    LugardeParto = modelo.LugardeParto,
                    EstatusMigratorio = modelo.EstatusMigratorio,
                    Desplazado = modelo.Desplazado,
                    Discapacitado = modelo.Discapacitado,
                    Fallecido = modelo.Fallecido,
                    VictimaConflictoArmado = modelo.VictimaConflictoArmado,
                    Estudia = modelo.Estudia,
                    PaisResidencia = modelo.PaisResidencia,
                    DepartamentoResidencia = modelo.DepartamentoResidencia,
                    MunicipioResidencia = modelo.MunicipioResidencia,
                    ComunaLocalidad = modelo.ComunaLocalidad,
                    Area = modelo.Area,
                    Celular = modelo.Celular,
                    Email = modelo.Email,
                    AutorizaLlamadas = modelo.AutorizaLlamadas,
                    AutorizaEnvioCorreo = modelo.AutorizaEnvioCorreo,
                    Relacion = modelo.Relacion,

                    // ANTECEDENTES MÉDICOS (Paso 3)
                    ArrayAntecedentes = modelo.ArrayAntecedentes,
                    EnfermedadContraindicacionVacuna = modelo.EnfermedadContraindicacionVacuna,
                    ReaccionBiologico = modelo.ReaccionBiologico,

                    // CONDICIÓN USUARIO/A (Paso 4)
                    CondicionUsuariaId = modelo.CondicionUsuariaId,

                    // MÉDICO/CUIDADOR (Paso 5)
                    MadreCuidador = modelo.MadreCuidador,
                    TipoIdentificacionCuidador = modelo.TipoIdentificacionCuidador,
                    NumeroDocumentoCuidador = modelo.NumeroDocumentoCuidador,
                    PrimerNombreCuidador = modelo.PrimerNombreCuidador,
                    SegundoNombreCuidador = modelo.SegundoNombreCuidador,
                    PrimerApellidoCuidador = modelo.PrimerApellidoCuidador,
                    SegundoApellidoCuidador = modelo.SegundoApellidoCuidador,
                    EmailCuidador = modelo.EmailCuidador,
                    TelefonoCuidador = modelo.TelefonoCuidador,
                    CelularCuidador = modelo.CelularCuidador,
                    RegimenAfiliacionCuidador = modelo.RegimenAfiliacionCuidador,
                    PertenenciaEtnicaIdCuidador = modelo.PertenenciaEtnicaIdCuidador,
                    EstadoDesplazadoCuidador = modelo.EstadoDesplazadoCuidador,
                    ParentescoCuidador = modelo.ParentescoCuidador,

                    // ESQUEMA VACUNACIÓN (Paso 6)
                    TipoCarnetId = modelo.TipoCarnetId,
                    Vacuna = modelo.Vacuna, 
                    Observaciones = modelo.Observaciones,
                    Dosis = modelo.Dosis,
                    FechaAplicacion = modelo.FechaRegistro ?? DateTime.Now,
                    FechaRegistro = modelo.FechaRegistro ?? DateTime.Now,
                    // RESPONSABLE (Paso 7)
                    Responsable = modelo.Responsable,
                    IngresoPAIWEB = modelo.IngresoPAIWEB,
                    CentroSaludResponsable = modelo.CentroSaludResponsable,

                    // CAMPOS DE CONTROL
                    NotasFinales = modelo.NotasFinales,
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    FechaAtencion = modelo.FechaAtencion,
                    EsquemaCompleto = modelo.EsquemaCompleto,
                    UsuarioCreadorId = modelo.UsuarioCreadorId ?? 1 // Valor por defecto temporalmente
                };

                // Guarda en base de datos
                _context.RegistrosVacunacion.Add(entidad);
                await _context.SaveChangesAsync(); 
                return Json(new { success = true, message = "Registro guardado correctamente", id = entidad.Id });
            }
            catch (Exception ex)
            { 
                return Json(new { success = false, message = "Ocurrió un error al guardar el registro: " + ex.Message });
            }
        }
    }
}