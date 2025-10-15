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
            var sql = $@"
        BEGIN TRANSACTION;
        DECLARE @Ultimo NVARCHAR(50);
        SELECT TOP 1 @Ultimo = Consecutivo  
        FROM RegistrosVacunacion WITH (UPDLOCK, HOLDLOCK)
        WHERE Consecutivo LIKE 'VAC-{año}-%' AND EmpresaId = @EmpresaId
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

            // Agregar el parámetro @EmpresaId
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@EmpresaId";
            parameter.Value = CurrentEmpresaId;
            command.Parameters.Add(parameter);

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
                Console.WriteLine($"GetDosisByVacuna llamado con vacunaId={vacunaId}, fechaNacimiento={fechaNacimiento}");
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

                // Agrupar por texto de dosis para evitar duplicados
                var result = filtrados
                    .GroupBy(f => f.DosisText)
                    .Select(g => new
                    {
                        value = g.First().Id,   // puedes cambiar a text si prefieres enviar el texto como value
                        text = g.Key
                    })
                    .OrderBy(x => x.text)
                    .ToList();

                return Json(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error en GetDosisByVacuna: " + ex.Message);
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
                // Log de los datos recibidos del request ANTES del model binding

                // Crear un modelo para capturar todos los datos del formulario
                var json = System.Text.Json.JsonSerializer.Serialize(datos);
                 Console.WriteLine("Modelo deserializado: " + json);
                 Console.WriteLine("Modelo currentEmpresaId: " + CurrentEmpresaId);
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
                    Vacuna = modelo.Vacuna,
                    Observaciones = modelo.Observaciones,
                    Dosis = modelo.Dosis,
                    FechaAplicacion = modelo.FechaRegistro ?? DateTime.Now,
                    FechaRegistro = modelo.FechaRegistro ?? DateTime.Now,
                    // RESPONSABLE (Paso 7)
                    Responsable = modelo.Responsable,
                    IngresoPAIWEB = modelo.IngresoPAIWEB,
                    CentroSaludResponsable = modelo.CentroSaludResponsable,
                    MarcarComoPerdida = modelo.MarcarComoPerdida,
                    MotivoPerdida = modelo.MotivoPerdida,
                    EmpresaId = CurrentEmpresaId,
                    // CAMPOS DE CONTROL
                    NotasFinales = modelo.NotasFinales,
                    Estado = true,
                    FechaCreacion = DateTime.Now,
                    FechaAtencion = modelo.FechaAtencion,
                    EsquemaCompleto = modelo.EsquemaCompleto,
                    UsuarioCreadorId = modelo.UsuarioCreadorId ?? 1 // Valor por defecto temporalmente
                };

                // Guarda en base de datos
                Console.WriteLine("Entidad a guardar: " + JsonSerializer.Serialize(entidad));
                _context.RegistrosVacunacion.Add(entidad);
                await _context.SaveChangesAsync();
                await GuardarAntecedentesMedicos(entidad.Id, modelo.ArrayAntecedentes, modelo.NumeroDocumento);
                return Json(new { success = true, message = "Registro guardado correctamente", id = entidad.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ocurrió un error al guardar el registro: " + ex.Message });
            }
        }

        // Método para procesar y guardar los antecedentes médicos
        private async Task GuardarAntecedentesMedicos(int registroVacunacionId, string? arrayAntecedentes, string numeroDocumentoPaciente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(arrayAntecedentes))
                    return;

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
                        FechaRegistro = antecedente.FechaRegistro,
                        Tipo = antecedente.Tipo,
                        Descripcion = antecedente.Descripcion,
                        Observaciones = antecedente.Observaciones,
                        Activo = antecedente.Activo,
                        NumeroDocumentoPaciente = numeroDocumentoPaciente,
                        FechaCreacion = DateTime.Now,
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
        NombreCompleto = $"{registro.PrimerNombre} {registro.SegundoNombre} {registro.PrimerApellido} {registro.SegundoApellido}".Trim()
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
    }
}