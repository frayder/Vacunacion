using Microsoft.AspNetCore.Mvc;
using Highdmin.Data;
using Highdmin.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Highdmin.Models;
using System.Text.Json.Serialization;
using System.Text.Json;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class RegistroVacunacionController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public RegistroVacunacionController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService, authorizationService)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Consultamos todos los registros de vacunación desde la base de datos

            var (redirect, permissions) = await ValidateAndGetPermissionsAsync("RegistroVacunacion", "Read");
            if (redirect != null) return redirect;

            var registros = await _context.RegistrosVacunacion
                .Where(r => r.EmpresaId == CurrentEmpresaId)
                .OrderByDescending(r => r.FechaRegistro)
                .Select(r => new RegistroVacunacionItemViewModel
                {
                    Id = r.Id,
                    TipoDocumento = r.TipoDocumento,
                    NumeroDocumento = r.NumeroDocumento,
                    NombreCompleto = r.PrimerNombre + " " + r.SegundoNombre + " " + r.PrimerApellido + " " + r.SegundoApellido,
                    FechaNacimiento = r.FechaNacimiento,
                    IngresoPAIWEB = r.IngresoPAIWEB,
                    Genero = r.Genero,
                    Telefono = r.Telefono,
                    Vacuna = r.Vacuna,
                    FechaRegistro = r.FechaRegistro,
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
                Registros = registros,
                // Agregar permisos al ViewModel
                CanCreate = permissions["Create"],
                CanUpdate = permissions["Update"],
                CanDelete = permissions["Delete"]

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
            var prefijo = $"VAC-{año}-";

            // Obtener el último consecutivo del año actual
            var ultimoConsecutivo = await _context.RegistrosVacunacion
                .Where(r => r.Consecutivo.StartsWith(prefijo) && r.EmpresaId == CurrentEmpresaId)
                .OrderByDescending(r => r.Consecutivo)
                .Select(r => r.Consecutivo)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(ultimoConsecutivo))
            {
                return $"{prefijo}000001";
            }

            // Extraer el número del consecutivo
            var numeroStr = ultimoConsecutivo.Substring(prefijo.Length);
            if (int.TryParse(numeroStr, out int numero))
            {
                numero++;
                return $"{prefijo}{numero:D6}";
            }

            // Si hay algún problema con el parsing, generar con timestamp
            var timestamp = DateTime.Now.ToString("HHmmss");
            return $"VAC-{año}-{timestamp}";
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

        // API endpoints para cargar datos de los dropdowns
        [HttpGet]
        public async Task<IActionResult> GetInsumos()
        {
            try
            {
                var insumos = await _context.Insumos
                    .Where(a => a.Estado && a.EmpresaId == CurrentEmpresaId) // filtrar por empresa si aplica
                    .OrderBy(a => a.Nombre)
                    .Select(a => new
                    {
                        value = a.Id,
                        text = a.Nombre,
                        codigo = a.Codigo,
                        tipo = a.Tipo,
                        descripcion = a.Descripcion,
                        rangoDosis = a.RangoDosis,
                        // Incluir configuraciones de rango activas
                        configuracionesRango = a.ConfiguracionesRango
                            .Where(cr => cr.Estado)
                            .Select(cr => new
                            {
                                id = cr.Id,
                                edadMinima = cr.EdadMinima,
                                edadMaxima = cr.EdadMaxima,
                                unidadMedidaEdadMinima = cr.UnidadMedidaEdadMinima,
                                unidadMedidaEdadMaxima = cr.UnidadMedidaEdadMaxima,
                                dosis = cr.Dosis,
                                descripcionRango = cr.DescripcionRango
                            })
                            .ToList()
                    })
                    .ToListAsync();

                return Json(insumos);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar insumos: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDosisByVacuna(int vacunaId, string fechaNacimiento)
        {
            try
            {
                if (vacunaId <= 0)
                    return Json(new { error = "vacunaId inválido" });
                if (string.IsNullOrEmpty(fechaNacimiento))
                    return Json(new { error = "fechaNacimiento es requerida" });

                if (!DateTime.TryParse(fechaNacimiento, out var fechaNac))
                    return Json(new { error = "fechaNacimiento inválida" });

                // Edad del paciente en días (referencia: hoy)
                var hoy = DateTime.UtcNow.Date;
                var edadDays = (hoy - fechaNac.Date).TotalDays;

                // Helper local para convertir un valor+unidad a días (double)
                double ConvertToDays(int valor, string unidad)
                {
                    if (string.IsNullOrWhiteSpace(unidad)) unidad = "Anos";
                    unidad = unidad.Trim().ToLowerInvariant();
                    // Soportar variantes comunes
                    if (unidad == "dias" || unidad == "d") // dias, d
                        return valor;
                    if (unidad == "meses" || unidad == "mes") // meses, mes
                        return valor * 30.436875; // promedio de días por mes
                    if (unidad == "anos" || unidad == "a") // años, años, y
                        return valor * 365.2425; // promedio con años bisiestos
                    return valor * 365.2425;
                }

                // Obtener las configuraciones activas para la vacuna (insumo)
                var configs = await _context.ConfiguracionesRangoInsumo
                    .Where(cr => cr.Estado && cr.InsumoId == vacunaId)
                    .Select(cr => new
                    {
                        Id = cr.Id,
                        DosisText = (cr.Dosis ?? cr.DescripcionRango ?? ("Dosis " + cr.Id)).Trim(),
                        DescripcionRango = cr.DescripcionRango ?? "",
                        EdadMin = cr.EdadMinima,
                        EdadMax = cr.EdadMaxima,
                        UnidadMin = cr.UnidadMedidaEdadMinima,
                        UnidadMax = cr.UnidadMedidaEdadMaxima
                    })
                    .ToListAsync();

                // Filtrar según edad del paciente (convertimos límites a días y comparamos con edadDays)
                var filtrados = configs
                    .Where(c =>
                    {
                        try
                        {
                            var minDays = ConvertToDays(c.EdadMin, c.UnidadMin);
                            var maxDays = ConvertToDays(c.EdadMax, c.UnidadMax);
                            // Si por alguna razón max < min, invertir o rechazar; aquí ignoramos esa configuración
                            if (maxDays < minDays) return false;

                            return edadDays >= minDays && edadDays <= maxDays;
                        }
                        catch
                        {
                            return false;
                        }
                    })
                    .ToList();


                return Json(filtrados);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar dosis: " + ex.Message });
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

                // Crear un modelo para capturar todos los datos del formulario
                var json = System.Text.Json.JsonSerializer.Serialize(datos);

                // Configuramos las opciones para que acepte números entre comillas Y maneje booleanos flexiblemente
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString,
                    Converters =
                    {
                        new JsonBooleanConverter(),
                        new JsonNullableBooleanConverter()
                    }
                };

                var modelo = JsonSerializer.Deserialize<RegistroVacunacionItemViewModel>(json, options);

                if (modelo == null)
                {
                    return Json(new { success = false, message = "Error al procesar los datos del formulario" });
                }


                // Mapear todos los datos del ViewModel a la entidad del modelo de datos
                var Consecutivo = await GenerarConsecutivoAsync();
                var utcNow = DateTime.UtcNow;

                DateTime ConvertirAUtc(DateTime? fecha)
                {
                    if (!fecha.HasValue)
                        return utcNow;

                    var fechaValue = fecha.Value;

                    return fechaValue.Kind switch
                    {
                        DateTimeKind.Utc => fechaValue,
                        DateTimeKind.Local => fechaValue.ToUniversalTime(),
                        DateTimeKind.Unspecified => DateTime.SpecifyKind(fechaValue, DateTimeKind.Utc),
                        _ => DateTime.SpecifyKind(fechaValue, DateTimeKind.Utc)
                    };
                }

                DateTime? ConvertirAUtcNullable(DateTime? fecha)
                {
                    if (!fecha.HasValue)
                        return null;

                    return ConvertirAUtc(fecha);
                }

                var entidad = new RegistrosVacunacion
                {
                    // DATOS BÁSICOS (Paso 1)
                    Consecutivo = Consecutivo,
                    PrimerNombre = modelo.PrimerNombre ?? string.Empty,
                    SegundoNombre = modelo.SegundoNombre ?? string.Empty,
                    PrimerApellido = modelo.PrimerApellido ?? string.Empty,
                    SegundoApellido = modelo.SegundoApellido ?? string.Empty,
                    TipoDocumento = modelo.TipoDocumento ?? string.Empty,
                    NumeroDocumento = modelo.NumeroDocumento ?? string.Empty,
                    FechaNacimiento = ConvertirAUtc(modelo.FechaNacimiento),
                    Genero = modelo.Genero ?? string.Empty,
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
                    PesoInfante = modelo.PesoInfante,
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
                    Vacuna = modelo.Vacuna ?? "N/A", // Valor por defecto si no hay vacunas
                    Observaciones = modelo.Observaciones,
                    Dosis = modelo.Dosis,
                    FechaAplicacion = ConvertirAUtc(modelo.FechaRegistro),
                    FechaRegistro = ConvertirAUtc(modelo.FechaRegistro),

                    // RESPONSABLE (Paso 7)
                    Responsable = modelo.Responsable,
                    IngresoPAIWEB = modelo.IngresoPAIWEB ?? false,
                    CentroSaludResponsable = modelo.CentroSaludResponsable,
                    MarcarComoPerdida = modelo.MarcarComoPerdida ?? false,
                    MotivoPerdida = modelo.MotivoPerdida,
                    EmpresaId = CurrentEmpresaId,

                    // CAMPOS DE CONTROL
                    NotasFinales = modelo.NotasFinales,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaAtencion = ConvertirAUtcNullable(modelo.FechaAtencion),
                    EsquemaCompleto = modelo.EsquemaCompleto,
                    UsuarioCreadorId = modelo.UsuarioCreadorId ?? 1 // Valor por defecto temporalmente
                };

                // Guarda en base de datos
                _context.RegistrosVacunacion.Add(entidad);
                await _context.SaveChangesAsync();

                // Guardar antecedentes médicos 
                await GuardarAntecedentesMedicos(entidad.Id, modelo.ArrayAntecedentes, modelo.NumeroDocumento);

                // Guardar vacunas aplicadas 
                await GuardarVacunasAplicadas(entidad.Id, modelo.ArrayVacunasAplicadas, modelo.NumeroDocumento);

                return Json(new { success = true, message = "Registro guardado correctamente", id = entidad.Id });
            }
            catch (Exception ex)
            {
                // Log más detallado del error 
                return Json(new { success = false, message = "Ocurrió un error al guardar el registro: " + ex.Message });
            }
        }

        // NUEVO: Método para procesar y guardar las vacunas aplicadas
        private async Task GuardarVacunasAplicadas(int registroVacunacionId, string? arrayVacunasAplicadas, string numeroDocumentoPaciente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(arrayVacunasAplicadas))
                    return;

                DateTime ConvertirAUtc(DateTime fecha)
                {
                    return fecha.Kind switch
                    {
                        DateTimeKind.Utc => fecha,
                        DateTimeKind.Local => fecha.ToUniversalTime(),
                        DateTimeKind.Unspecified => DateTime.SpecifyKind(fecha, DateTimeKind.Utc),
                        _ => DateTime.SpecifyKind(fecha, DateTimeKind.Utc)
                    };
                }

                // Deserializar el array de vacunas desde JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var vacunas = JsonSerializer.Deserialize<List<VacunaAplicadaViewModel>>(arrayVacunasAplicadas, options);

                if (vacunas == null || !vacunas.Any())
                    return;

                // Crear las entidades de vacunas aplicadas
                var entidadesVacunas = new List<VacunaAplicada>();

                foreach (var vacuna in vacunas)
                {
                    var entidadVacuna = new VacunaAplicada
                    {
                        RegistroVacunacionId = registroVacunacionId,
                        InsumoId = vacuna.InsumoId,
                        NombreVacuna = vacuna.NombreVacuna,
                        Dosis = vacuna.Dosis,
                        LoteVacuna = vacuna.LoteVacuna,
                        Jeringa = vacuna.Jeringa,
                        LoteJeringa = vacuna.LoteJeringa,
                        LoteDiluyente = vacuna.LoteDiluyente,
                        Gotero = vacuna.Gotero,
                        NumeroFrascos = vacuna.NumeroFrascos,
                        Observaciones = vacuna.Observaciones,
                        MarcarComoPerdida = vacuna.MarcarComoPerdida,
                        MotivoPerdida = vacuna.MotivoPerdida,
                        FechaAplicacion = ConvertirAUtc(vacuna.FechaAplicacion),
                        Activo = vacuna.Activo,
                        NumeroDocumentoPaciente = numeroDocumentoPaciente,
                        FechaCreacion = DateTime.UtcNow,
                        EmpresaId = CurrentEmpresaId
                    };

                    entidadesVacunas.Add(entidadVacuna);
                }

                // Guardar todas las vacunas en la base de datos
                if (entidadesVacunas.Any())
                {
                    _context.VacunasAplicadas.AddRange(entidadesVacunas);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log del error pero no fallar la operación principal
                Console.WriteLine($"Error al guardar vacunas aplicadas: {ex.Message}");
                throw; // Re-lanzar para que se maneje en el método principal
            }
        }

        // Método para procesar y guardar los antecedentes médicos
        private async Task GuardarAntecedentesMedicos(int registroVacunacionId, string? arrayAntecedentes, string numeroDocumentoPaciente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(arrayAntecedentes))
                    return;

                DateTime ConvertirAUtc(DateTime fecha)
                {
                    return fecha.Kind switch
                    {
                        DateTimeKind.Utc => fecha,
                        DateTimeKind.Local => fecha.ToUniversalTime(),
                        DateTimeKind.Unspecified => DateTime.SpecifyKind(fecha, DateTimeKind.Utc),
                        _ => DateTime.SpecifyKind(fecha, DateTimeKind.Utc)
                    };
                }

                // Deserializar el array de antecedentes desde JSON
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                };

                var antecedentes = JsonSerializer.Deserialize<List<AntecedenteMedicoViewModel>>(arrayAntecedentes, options);

                if (antecedentes == null || !antecedentes.Any())
                    return;

                // Crear las entidades de antecedentes médicos
                var entidadesAntecedentes = new List<AntecedenteMedico>();

                foreach (var antecedente in antecedentes)
                {
                    var entidadAntecedente = new AntecedenteMedico
                    {
                        RegistroVacunacionId = registroVacunacionId,
                        FechaRegistro = ConvertirAUtc(antecedente.FechaRegistro),
                        Tipo = antecedente.Tipo,
                        Descripcion = antecedente.Descripcion,
                        Observaciones = antecedente.Observaciones,
                        Activo = antecedente.Activo,
                        NumeroDocumentoPaciente = numeroDocumentoPaciente,
                        FechaCreacion = DateTime.UtcNow,
                        EmpresaId = CurrentEmpresaId
                    };

                    entidadesAntecedentes.Add(entidadAntecedente);
                }

                // Guardar todos los antecedentes en la base de datos
                if (entidadesAntecedentes.Any())
                {
                    _context.AntecedentesMedicos.AddRange(entidadesAntecedentes);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log del error pero no fallar la operación principal
                // En un entorno de producción, registrar este error en un sistema de logging
                Console.WriteLine($"Error al guardar antecedentes médicos: {ex.Message}");
                throw; // Re-lanzar para que se maneje en el método principal
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var registro = await _context.RegistrosVacunacion
                .Include(r => r.Aseguradora)
                .Include(r => r.RegimenAfiliacion)
                .Include(r => r.PertenenciaEtnica)
                .Include(r => r.CentroAtencion)
                .Include(r => r.CondicionUsuaria)
                .Include(r => r.TipoCarnet)
                .Include(r => r.VacunasAplicadas)
                .ThenInclude(va => va.Insumo)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registro == null)
            {
                return NotFound();
            }

            // Obtener nombres de régimen de afiliación y pertenencia étnica para el cuidador
            string? regimenAfiliacionCuidadorNombre = null;
            string? pertenenciaEtnicaCuidadorNombre = null;

            if (registro.RegimenAfiliacionCuidador.HasValue)
            {
                var regimenCuidador = await _context.RegimenesAfiliacion
                    .FirstOrDefaultAsync(r => r.Id == registro.RegimenAfiliacionCuidador.Value);
                regimenAfiliacionCuidadorNombre = regimenCuidador?.Nombre;
            }

            if (registro.PertenenciaEtnicaIdCuidador.HasValue)
            {
                var pertenenciaCuidador = await _context.PertenenciasEtnicas
                    .FirstOrDefaultAsync(p => p.Id == registro.PertenenciaEtnicaIdCuidador.Value);
                pertenenciaEtnicaCuidadorNombre = pertenenciaCuidador?.Nombre;
            }

            var viewModel = new RegistroVacunacionItemViewModel
            {
                Id = registro.Id,
                Consecutivo = registro.Consecutivo,
                FechaRegistro = registro.FechaRegistro,
                PrimerNombre = registro.PrimerNombre,
                SegundoNombre = registro.SegundoNombre,
                PrimerApellido = registro.PrimerApellido,
                SegundoApellido = registro.SegundoApellido,
                TipoDocumento = registro.TipoDocumento,
                NumeroDocumento = registro.NumeroDocumento,

                FechaNacimiento = registro.FechaNacimiento,
                Genero = registro.Genero,
                Telefono = registro.Telefono,
                Direccion = registro.Direccion,
                AseguradoraId = registro.AseguradoraId,
                RegimenAfiliacionId = registro.RegimenAfiliacionId,
                PertenenciaEtnicaId = registro.PertenenciaEtnicaId,
                CentroAtencionId = registro.CentroAtencionId,
                CondicionUsuariaId = registro.CondicionUsuariaId,
                TipoCarnetId = registro.TipoCarnetId,

                // Nombres de las entidades relacionadas
                AseguradoraNombre = registro.Aseguradora?.Nombre,
                RegimenAfiliacionNombre = registro.RegimenAfiliacion?.Nombre,
                PertenenciaEtnicaNombre = registro.PertenenciaEtnica?.Nombre,
                CentroAtencionNombre = registro.CentroAtencion?.Nombre,
                CondicionUsuariaNombre = registro.CondicionUsuaria?.Nombre,
                TipoCarnetNombre = registro.TipoCarnet?.Nombre,


                Vacuna = registro.Vacuna,
                Observaciones = registro.Observaciones,
                NotasFinales = registro.NotasFinales,
                FechaCreacion = registro.FechaCreacion,
                FechaModificacion = registro.FechaModificacion,
                UsuarioCreadorId = registro.UsuarioCreadorId,
                UsuarioModificadorId = registro.UsuarioModificadorId,
                FechaAtencion = registro.FechaAtencion,
                EsquemaCompleto = registro.EsquemaCompleto,
                Sexo = registro.Sexo,
                OrientacionSexual = registro.OrientacionSexual,
                EdadGestacional = registro.EdadGestacional,
                PesoInfante = registro.PesoInfante,
                PaisNacimiento = registro.PaisNacimiento,
                EstatusMigratorio = registro.EstatusMigratorio,
                Desplazado = registro.Desplazado,
                Discapacitado = registro.Discapacitado,
                Fallecido = registro.Fallecido,
                VictimaConflictoArmado = registro.VictimaConflictoArmado,
                Estudia = registro.Estudia,
                PaisResidencia = registro.PaisResidencia,
                DepartamentoResidencia = registro.DepartamentoResidencia,
                MunicipioResidencia = registro.MunicipioResidencia,
                ComunaLocalidad = registro.ComunaLocalidad,
                Area = registro.Area,
                Celular = registro.Celular,
                Email = registro.Email,
                AutorizaLlamadas = registro.AutorizaLlamadas,
                AutorizaEnvioCorreo = registro.AutorizaEnvioCorreo,
                Relacion = registro.Relacion,
                EnfermedadContraindicacionVacuna = registro.EnfermedadContraindicacionVacuna,
                ReaccionBiologico = registro.ReaccionBiologico,
                MadreCuidador = registro.MadreCuidador,
                TipoIdentificacionCuidador = registro.TipoIdentificacionCuidador,
                NumeroDocumentoCuidador = registro.NumeroDocumentoCuidador,
                PrimerNombreCuidador = registro.PrimerNombreCuidador,
                SegundoNombreCuidador = registro.SegundoNombreCuidador,
                PrimerApellidoCuidador = registro.PrimerApellidoCuidador,
                SegundoApellidoCuidador = registro.SegundoApellidoCuidador,
                EmailCuidador = registro.EmailCuidador,
                TelefonoCuidador = registro.TelefonoCuidador,
                CelularCuidador = registro.CelularCuidador,
                RegimenAfiliacionCuidador = registro.RegimenAfiliacionCuidador,
                PertenenciaEtnicaIdCuidador = registro.PertenenciaEtnicaIdCuidador,
                EstadoDesplazadoCuidador = registro.EstadoDesplazadoCuidador,
                ParentescoCuidador = registro.ParentescoCuidador,
                Dosis = registro.Dosis,
                Responsable = registro.Responsable,
                IngresoPAIWEB = registro.IngresoPAIWEB,
                CentroSaludResponsable = registro.CentroSaludResponsable,
                MotivoNoIngresoPAIWEB = registro.MotivoNoIngresoPAIWEB,
                NombreCompleto = $"{registro.PrimerNombre} {registro.SegundoNombre} {registro.PrimerApellido} {registro.SegundoApellido}".Trim(),
                RegimenAfiliacionCuidadorNombre = regimenAfiliacionCuidadorNombre,
                PertenenciaEtnicaCuidadorNombre = pertenenciaEtnicaCuidadorNombre,
                VacunasAplicadasList = registro.VacunasAplicadas
                    .Where(va => va.Activo)
                    .Select(va => new VacunaAplicadaItemViewModel
                    {
                        Id = va.Id,
                        NombreVacuna = va.NombreVacuna,
                        Dosis = va.Dosis,
                        LoteVacuna = va.LoteVacuna,
                        Jeringa = va.Jeringa,
                        LoteJeringa = va.LoteJeringa,
                        LoteDiluyente = va.LoteDiluyente,
                        Gotero = va.Gotero,
                        NumeroFrascos = va.NumeroFrascos,
                        Observaciones = va.Observaciones,
                        FechaAplicacion = va.FechaAplicacion,
                        MarcarComoPerdida = va.MarcarComoPerdida,
                        MotivoPerdida = va.MotivoPerdida,
                        InsumoNombre = va.Insumo?.Nombre,
                        InsumoCodigo = va.Insumo?.Codigo,
                        InsumoTipo = va.Insumo?.Tipo
                    })
                    .OrderByDescending(va => va.FechaAplicacion)
                    .ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var registro = await _context.RegistrosVacunacion
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registro == null)
            {
                return NotFound();
            }

            var viewModel = new RegistroVacunacionItemViewModel
            {
                Id = registro.Id,
                Consecutivo = registro.Consecutivo,
                FechaRegistro = registro.FechaRegistro,
                NombreCompleto = $"{registro.PrimerNombre} {registro.SegundoNombre} {registro.PrimerApellido} {registro.SegundoApellido}".Trim(),
                NumeroDocumento = registro.NumeroDocumento,
                TipoDocumento = registro.TipoDocumento,
                Vacuna = registro.Vacuna,
                FechaAtencion = registro.FechaAtencion,
                IngresoPAIWEB = registro.IngresoPAIWEB
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var registro = await _context.RegistrosVacunacion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (registro != null)
                {
                    _context.RegistrosVacunacion.Remove(registro);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "El registro de vacunación ha sido eliminado exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "No se encontró el registro a eliminar.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el registro: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetInsumosByTipo(string tipo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipo))
                {
                    return Json(new { error = "El tipo de insumo es requerido" });
                }

                var insumos = await _context.Insumos
                    .Where(a => a.Estado && a.EmpresaId == CurrentEmpresaId && a.Tipo.ToLower() == tipo.ToLower())
                    .OrderBy(a => a.Nombre)
                    .Select(a => new
                    {
                        value = a.Id,
                        text = a.Nombre,
                        codigo = a.Codigo,
                        tipo = a.Tipo,
                        descripcion = a.Descripcion
                    })
                    .ToListAsync();

                return Json(insumos);
            }
            catch (Exception ex)
            {
                return Json(new { error = "Error al cargar insumos: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJeringas()
        {
            return await GetInsumosByTipo("Jeringa");
        }
    }
}

// Convertidor personalizado para manejar valores booleanos desde JSON
public class JsonBooleanConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.String:
                string? stringValue = reader.GetString();
                if (bool.TryParse(stringValue, out bool boolValue))
                    return boolValue;
                if (stringValue?.ToLower() == "true" || stringValue == "1")
                    return true;
                if (stringValue?.ToLower() == "false" || stringValue == "0" || string.IsNullOrEmpty(stringValue))
                    return false;
                break;
            case JsonTokenType.Number:
                return reader.GetInt32() != 0;
            case JsonTokenType.Null:
                return false;
        }
        return false;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }
}

// Convertidor para booleanos nullable
public class JsonNullableBooleanConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.String:
                string? stringValue = reader.GetString();
                if (bool.TryParse(stringValue, out bool boolValue))
                    return boolValue;
                if (stringValue?.ToLower() == "true" || stringValue == "1" || stringValue?.ToLower() == "si")
                    return true;
                if (stringValue?.ToLower() == "false" || stringValue == "0" || stringValue?.ToLower() == "no")
                    return false;
                if (string.IsNullOrEmpty(stringValue))
                    return null;
                break;
            case JsonTokenType.Number:
                return reader.GetInt32() != 0;
            case JsonTokenType.Null:
                return null;
        }
        return null;
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}