using Microsoft.AspNetCore.Mvc;
using Highdmin.Data;
using Highdmin.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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
            // Lógica temporal - solo mostrar que se recibieron los datos
            if (ModelState.IsValid)
            {
                TempData["Success"] = "Registro recibido correctamente - implementar lógica de guardado";
                return RedirectToAction(nameof(Index));
            }

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarRegistroCompleto()
        {
            try
            {
                // Log inicial para debugging
                System.Diagnostics.Debug.WriteLine("=== INICIO GuardarRegistroCompleto ===");
                
                // Obtener todos los datos del formulario paso a paso desde el Request
                var form = Request.Form;
                System.Diagnostics.Debug.WriteLine($"Form.Count: {form.Count}");
                
                if (!form.Any())
                {
                    System.Diagnostics.Debug.WriteLine("No se encontraron datos en el formulario");
                    return Json(new { 
                        success = false, 
                        message = "No se recibieron datos del formulario"
                    });
                }

                // Helper para extraer valores de forma segura
                string GetFormValue(string key) 
                {
                    return form.ContainsKey(key) ? form[key].ToString().Trim() : string.Empty;
                }
                
                int? GetFormInt(string key) 
                {
                    var value = GetFormValue(key);
                    return int.TryParse(value, out int result) && result > 0 ? result : null;
                }
                
                DateTime? GetFormDate(string key) 
                {
                    var value = GetFormValue(key);
                    return DateTime.TryParse(value, out DateTime result) ? result : null;
                }

                // Log de todos los datos recibidos para debugging
                var todosLosCampos = new Dictionary<string, string>();
                foreach (var key in form.Keys)
                {
                    var valor = form[key].ToString();
                    todosLosCampos[key] = valor;
                    System.Diagnostics.Debug.WriteLine($"Campo recibido: {key} = {valor}");
                }

                // Crear el registro mapeando correctamente los campos del formulario
                var primerNombre = GetFormValue("PrimerNombre");
                var segundoNombre = GetFormValue("SegundoNombre");
                var primerApellido = GetFormValue("PrimerApellido");
                var segundoApellido = GetFormValue("SegundoApellido");
                
                // Construir nombre completo
                var nombresCompletos = $"{primerNombre} {segundoNombre}".Trim();
                var apellidosCompletos = $"{primerApellido} {segundoApellido}".Trim();
                var nombreCompleto = $"{nombresCompletos} {apellidosCompletos}".Trim();
                
                var registro = new
                {
                    // Generar consecutivo automático
                    Consecutivo = await GenerarConsecutivoAsync(),
                    
                    // Datos básicos del paciente (mapear desde los campos reales del formulario)
                    NombresApellidos = nombreCompleto,
                    TipoDocumento = GetFormValue("TipoDocumento"),
                    NumeroDocumento = GetFormValue("Documento"),
                    FechaNacimiento = GetFormDate("FechaNacimiento") ?? DateTime.Now.AddYears(-20),
                    Genero = GetFormValue("Genero"),
                    Telefono = GetFormValue("Telefono"),
                    Direccion = GetFormValue("Direccion"),
                    
                    // Datos de afiliación (usar nombres exactos de los campos del formulario)
                    AseguradoraId = GetFormInt("AseguradoraId"),
                    RegimenAfiliacionId = GetFormInt("RegimenAfiliacionId"),
                    PertenenciaEtnicaId = GetFormInt("PertenenciaEtnicaId"),
                    
                    // Datos de atención
                    CentroAtencionId = GetFormInt("CentroAtencionId"),
                    CondicionUsuariaId = GetFormInt("CondicionUsuariaId"),
                    TipoCarnetId = GetFormInt("TipoCarnetId"),
                    
                    // Datos de la vacuna (mapear desde los campos correctos del formulario)
                    Vacuna = GetFormValue("VacunaSeleccionada") ?? GetFormValue("Vacuna") ?? GetFormValue("TipoVacuna"),
                    NumeroDosis = GetFormValue("NumeroDosis") ?? GetFormValue("Dosis"),
                    FechaAplicacion = GetFormDate("FechaAplicacion") ?? GetFormDate("FechaRegistro") ?? DateTime.Now,
                    Lote = GetFormValue("LoteVacuna") ?? GetFormValue("Lote"),
                    Laboratorio = GetFormValue("Laboratorio"),
                    ViaAdministracion = GetFormValue("ViaAdministracion"),
                    SitioAplicacion = GetFormValue("SitioAplicacion"),
                    
                    // Datos del responsable
                    Vacunador = GetFormValue("Vacunador"),
                    RegistroProfesional = GetFormValue("RegistroProfesional"),
                    
                    // Observaciones (mapear desde diferentes posibles campos)
                    Observaciones = GetFormValue("ObservacionesVacuna") ?? GetFormValue("Observaciones"),
                    NotasFinales = GetFormValue("NotasFinales"),
                    
                    // Campos de auditoría
                    Estado = true,
                    FechaCreacion = DateTime.Now
                };

                // Validar campos obligatorios antes de insertar
                var errores = new List<string>();
                var debug = new List<string>();
                
                // Validar y debuggear cada campo obligatorio
                if (string.IsNullOrWhiteSpace(registro.NombresApellidos))
                {
                    errores.Add("Nombres y Apellidos");
                    debug.Add($"NombresApellidos vacío. PrimerNombre={primerNombre}, SegundoNombre={segundoNombre}, PrimerApellido={primerApellido}, SegundoApellido={segundoApellido}");
                }
                if (string.IsNullOrWhiteSpace(registro.TipoDocumento))
                {
                    errores.Add("Tipo de Documento");
                    debug.Add($"TipoDocumento vacío. Valor recibido: '{registro.TipoDocumento}'");
                }
                if (string.IsNullOrWhiteSpace(registro.NumeroDocumento))
                {
                    errores.Add("Número de Documento");
                    debug.Add($"NumeroDocumento vacío. Valor recibido: '{registro.NumeroDocumento}'");
                }
                if (string.IsNullOrWhiteSpace(registro.Genero))
                {
                    errores.Add("Género");
                    debug.Add($"Genero vacío. Valor recibido: '{registro.Genero}'");
                }
                if (string.IsNullOrWhiteSpace(registro.Vacuna))
                {
                    errores.Add("Vacuna");
                    debug.Add($"Vacuna vacía. VacunaSeleccionada='{GetFormValue("VacunaSeleccionada")}', Vacuna='{GetFormValue("Vacuna")}', TipoVacuna='{GetFormValue("TipoVacuna")}'");
                }
                if (string.IsNullOrWhiteSpace(registro.NumeroDosis))
                {
                    errores.Add("Número de Dosis");
                    debug.Add($"NumeroDosis vacío. Valor recibido: '{registro.NumeroDosis}'");
                }

                if (errores.Any())
                {
                    var debugInfo = string.Join("; ", debug);
                    System.Diagnostics.Debug.WriteLine($"Validación fallida: {debugInfo}");
                    
                    return Json(new { 
                        success = false, 
                        message = $"Campos obligatorios faltantes: {string.Join(", ", errores)}",
                        debug = debugInfo,
                        camposRecibidos = todosLosCampos.Keys.ToArray()
                    });
                }

                // Usar SqlParameter para evitar problemas con DBNull
                var parameters = new[]
                {
                    new SqlParameter("@Consecutivo", registro.Consecutivo ?? ""),
                    new SqlParameter("@NombresApellidos", registro.NombresApellidos ?? ""),
                    new SqlParameter("@TipoDocumento", registro.TipoDocumento ?? ""),
                    new SqlParameter("@NumeroDocumento", registro.NumeroDocumento ?? ""),
                    new SqlParameter("@FechaNacimiento", registro.FechaNacimiento),
                    new SqlParameter("@Genero", registro.Genero ?? ""),
                    new SqlParameter("@Telefono", (object?)registro.Telefono ?? DBNull.Value),
                    new SqlParameter("@Direccion", (object?)registro.Direccion ?? DBNull.Value),
                    new SqlParameter("@AseguradoraId", (object?)registro.AseguradoraId ?? DBNull.Value),
                    new SqlParameter("@RegimenAfiliacionId", (object?)registro.RegimenAfiliacionId ?? DBNull.Value),
                    new SqlParameter("@PertenenciaEtnicaId", (object?)registro.PertenenciaEtnicaId ?? DBNull.Value),
                    new SqlParameter("@CentroAtencionId", (object?)registro.CentroAtencionId ?? DBNull.Value),
                    new SqlParameter("@CondicionUsuariaId", (object?)registro.CondicionUsuariaId ?? DBNull.Value),
                    new SqlParameter("@TipoCarnetId", (object?)registro.TipoCarnetId ?? DBNull.Value),
                    new SqlParameter("@Vacuna", registro.Vacuna ?? ""),
                    new SqlParameter("@NumeroDosis", registro.NumeroDosis ?? ""),
                    new SqlParameter("@FechaAplicacion", registro.FechaAplicacion),
                    new SqlParameter("@Lote", (object?)registro.Lote ?? DBNull.Value),
                    new SqlParameter("@Laboratorio", (object?)registro.Laboratorio ?? DBNull.Value),
                    new SqlParameter("@ViaAdministracion", (object?)registro.ViaAdministracion ?? DBNull.Value),
                    new SqlParameter("@SitioAplicacion", (object?)registro.SitioAplicacion ?? DBNull.Value),
                    new SqlParameter("@Vacunador", (object?)registro.Vacunador ?? DBNull.Value),
                    new SqlParameter("@RegistroProfesional", (object?)registro.RegistroProfesional ?? DBNull.Value),
                    new SqlParameter("@Observaciones", (object?)registro.Observaciones ?? DBNull.Value),
                    new SqlParameter("@NotasFinales", (object?)registro.NotasFinales ?? DBNull.Value),
                    new SqlParameter("@Estado", registro.Estado),
                    new SqlParameter("@FechaCreacion", registro.FechaCreacion)
                };

                var sql = @"
                    INSERT INTO RegistroVacunacion 
                    (Consecutivo, NombresApellidos, TipoDocumento, NumeroDocumento, FechaNacimiento, Genero, 
                     Telefono, Direccion, AseguradoraId, RegimenAfiliacionId, PertenenciaEtnicaId, 
                     CentroAtencionId, CondicionUsuariaId, TipoCarnetId, Vacuna, NumeroDosis, FechaAplicacion, 
                     Lote, Laboratorio, ViaAdministracion, SitioAplicacion, Vacunador, RegistroProfesional, 
                     Observaciones, NotasFinales, Estado, FechaCreacion)
                    VALUES 
                    (@Consecutivo, @NombresApellidos, @TipoDocumento, @NumeroDocumento, @FechaNacimiento, @Genero,
                     @Telefono, @Direccion, @AseguradoraId, @RegimenAfiliacionId, @PertenenciaEtnicaId,
                     @CentroAtencionId, @CondicionUsuariaId, @TipoCarnetId, @Vacuna, @NumeroDosis, @FechaAplicacion,
                     @Lote, @Laboratorio, @ViaAdministracion, @SitioAplicacion, @Vacunador, @RegistroProfesional,
                     @Observaciones, @NotasFinales, @Estado, @FechaCreacion)";

                // Ejecutar el INSERT con parámetros nombrados
                var result = await _context.Database.ExecuteSqlRawAsync(sql, parameters);

                System.Diagnostics.Debug.WriteLine($"Registro guardado con consecutivo: {registro.Consecutivo}");
                System.Diagnostics.Debug.WriteLine("=== FIN GuardarRegistroCompleto ===");

                return Json(new { 
                    success = true, 
                    message = "Registro de vacunación guardado exitosamente en la base de datos",
                    consecutivo = registro.Consecutivo,
                    datosGuardados = form.Count
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en GuardarRegistroCompleto: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                
                return Json(new { 
                    success = false, 
                    message = "Error al guardar el registro: " + ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        // Método auxiliar para generar consecutivo
        private async Task<string> GenerarConsecutivoAsync()
        {
            try
            {
                // Usar una consulta SQL directa para obtener el último ID
                var sql = "SELECT TOP 1 Id FROM RegistroVacunacion ORDER BY Id DESC";
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sql;
                    await _context.Database.OpenConnectionAsync();
                    var result = await command.ExecuteScalarAsync();
                    await _context.Database.CloseConnectionAsync();
                    
                    var ultimoId = result != null ? Convert.ToInt32(result) : 0;
                    var siguienteNumero = ultimoId + 1;
                    return $"VAC-{DateTime.Now.Year}-{siguienteNumero:D6}";
                }
            }
            catch
            {
                // Si hay error o no hay registros, empezar desde 1
                return $"VAC-{DateTime.Now.Year}-{1:D6}";
            }
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
    }
}