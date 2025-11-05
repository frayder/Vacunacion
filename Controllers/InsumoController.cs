using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;
using System.Security.Claims;
using Newtonsoft.Json;

namespace Highdmin.Controllers
{
    public class InsumoController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;
        private readonly IImportExportService _importExportService;
        private readonly IEntityConfigurationService _configurationService;
        private readonly IDataPersistenceService _persistenceService;

        public InsumoController(
            ApplicationDbContext context,
            IEmpresaService empresaService,
            AuthorizationService authorizationService,
            IImportExportService importExportService,
            IEntityConfigurationService configurationService,
            IDataPersistenceService persistenceService)
            : base(empresaService, authorizationService)
        {
            _context = context;
            _importExportService = importExportService;
            _configurationService = configurationService;
            _persistenceService = persistenceService;
        }

        // GET: Insumo
        public async Task<IActionResult> Index()
        {
            try
            {
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("Insumos", "Read");
                if (redirect != null) return redirect;

                var insumos = await _context.Insumos
                    .Where(i => i.EmpresaId == CurrentEmpresaId)
                    .OrderBy(i => i.Codigo)
                    .Select(i => new InsumoItemViewModel
                    {
                        Id = i.Id,
                        Codigo = i.Codigo,
                        Nombre = i.Nombre,
                        Descripcion = i.Descripcion,
                        Tipo = i.Tipo,
                        RangoDosis = i.RangoDosis,
                        Estado = i.Estado,
                        FechaCreacion = i.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new InsumoIndexViewModel
                {
                    Insumos = insumos,
                    CanCreate = permissions["Create"],
                    CanRead = permissions["Read"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los insumos: " + ex.Message;
                return View(new InsumoIndexViewModel());
            }
        }

        // GET: Insumo/Exportar
        public async Task<IActionResult> Exportar()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasExportPermission = await _authorizationService.HasPermissionAsync(userId, "Insumo", "Export") ||
                                    await _authorizationService.HasPermissionAsync(userId, "Insumo", "Read");

            if (!hasExportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para exportar insumos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var insumos = await _context.Insumos
                    .Where(i => i.EmpresaId == CurrentEmpresaId)
                    .OrderBy(i => i.Codigo)
                    .ToListAsync();

                var exportConfig = _configurationService.GetExportConfiguration<Insumo>();
                var excelData = await _importExportService.ExportToExcelAsync(insumos, exportConfig);
                var fileName = $"{exportConfig.FileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al exportar los insumos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Insumo/ImportarPlantilla
        public async Task<IActionResult> ImportarPlantilla()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "Insumo", "Import") ||
                                    await _authorizationService.HasPermissionAsync(userId, "Insumo", "Create");

            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar insumos.";
                return RedirectToAction(nameof(Index));
            }

            return View(new ImportarInsumoViewModel());
        }

        // GET: Insumo/DescargarPlantilla
        [HttpGet]
        [ActionName("DescargarPlantilla")]
        public IActionResult DescargarPlantilla()
        {
            try
            {
                var importConfig = _configurationService.GetImportConfiguration<InsumoItemViewModel>();
                var templateData = _importExportService.GenerateImportTemplate(importConfig);
                var fileName = $"Plantilla_{importConfig.SheetName.Replace(" ", "")}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(templateData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar la plantilla: " + ex.Message;
                return RedirectToAction(nameof(ImportarPlantilla));
            }
        }

        // POST: Insumo/ImportarPlantilla
        [HttpPost]
        [ActionName("ImportarPlantilla")]
        public async Task<IActionResult> ImportarPlantilla(ImportarInsumoViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasImportPermission = await _authorizationService.HasPermissionAsync(userId, "Insumo", "Import") ||
                                    await _authorizationService.HasPermissionAsync(userId, "Insumo", "Create");

            if (!hasImportPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para importar insumos.";
                return RedirectToAction(nameof(Index));
            }

            if (model.ArchivoExcel == null || model.ArchivoExcel.Length == 0)
            {
                ModelState.AddModelError("ArchivoExcel", "Debe seleccionar un archivo Excel.");
                return View(model);
            }

            try
            {
                var importConfig = _configurationService.GetImportConfiguration<InsumoItemViewModel>();
                var importResult = await _importExportService.ImportFromExcelAsync(model.ArchivoExcel, importConfig);

                if (importResult.HasErrors)
                {
                    ViewBag.Errores = importResult.Errors;
                    return View(model);
                }

                if (!importResult.Data.Any())
                {
                    ModelState.AddModelError("", "No se encontraron datos válidos para importar.");
                    return View(model);
                }

                HttpContext.Session.SetString("InsumosCargados", JsonConvert.SerializeObject(importResult.Data));
                model.InsumosCargados = importResult.Data;
                TempData["Success"] = $"Se procesaron {importResult.Data.Count} insumos correctamente. Revise los datos y confirme la importación.";

                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al procesar el archivo: " + ex.Message);
                return View(model);
            }
        }

        // POST: Insumo/GuardarInsumosImportados
        [HttpPost]
        public async Task<IActionResult> GuardarInsumosImportados()
        {
            var json = HttpContext.Session.GetString("InsumosCargados");
            if (string.IsNullOrEmpty(json))
            {
                TempData["Error"] = "No hay datos para importar.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var insumosCargados = JsonConvert.DeserializeObject<List<InsumoItemViewModel>>(json);
                if (insumosCargados == null || !insumosCargados.Any())
                {
                    TempData["Error"] = "No hay datos para importar.";
                    return RedirectToAction(nameof(Index));
                }

                var totalProcessed = await _persistenceService.SaveImportedDataAsync<Insumo, InsumoItemViewModel>(
                    insumosCargados,
                    CurrentEmpresaId,
                    // Create mapper
                    viewModel => new Insumo
                    {
                        Codigo = viewModel.Codigo.ToUpper(),
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.UtcNow,
                        EmpresaId = CurrentEmpresaId
                    },
                    // Update mapper
                    (viewModel, existing) =>
                    {
                        existing.Nombre = viewModel.Nombre;
                        existing.Descripcion = viewModel.Descripcion;
                        existing.Estado = viewModel.Estado;
                        return existing;
                    },
                    // Find existing
                    (viewModel, dbSet) => dbSet.FirstOrDefault(i =>
                        i.EmpresaId == CurrentEmpresaId &&
                        i.Codigo.ToUpper() == viewModel.Codigo.ToUpper())
                );

                HttpContext.Session.Remove("InsumosCargados");
                TempData["Success"] = $"Importación completada: {totalProcessed} insumos procesados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar los insumos: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Insumo/Create
        public IActionResult Create()
        {
            return View(new InsumoCreateViewModel());
        }

        // POST: Insumo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InsumoCreateViewModel viewModel)
        {
            Console.WriteLine("Received ViewModel: " + ModelState.IsValid);
            if (ModelState.IsValid)
            {
                try
                {
                    // Usar la estrategia de ejecución de Entity Framework para manejar transacciones
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();

                        try
                        {
                            // Verificar si el código ya existe
                            var existeCodigo = await _context.Insumos
                                .AnyAsync(i => i.Codigo == viewModel.Codigo && i.EmpresaId == CurrentEmpresaId);
                            Console.WriteLine("Existe código: " + existeCodigo);
                            if (existeCodigo)
                            {
                                throw new InvalidOperationException("Ya existe un insumo con este código.");
                            }



                            // Validar configuraciones de rango
                            for (int i = 0; i < viewModel.ConfiguracionesRango?.Count; i++)
                            {
                                var config = viewModel.ConfiguracionesRango[i];

                                // Convertir edades a días para comparar correctamente
                                var edadMinEnDias = ConvertirADias(config.EdadMinima, config.UnidadMedidaEdadMinima);
                                var edadMaxEnDias = ConvertirADias(config.EdadMaxima, config.UnidadMedidaEdadMaxima);

                                if (edadMinEnDias >= edadMaxEnDias)
                                {
                                    throw new InvalidOperationException("La edad máxima debe ser mayor que la edad mínima considerando las unidades de medida.");
                                }

                                if (string.IsNullOrEmpty(config.UnidadMedidaEdadMinima) ||
                                    string.IsNullOrEmpty(config.UnidadMedidaEdadMaxima))
                                {
                                    throw new InvalidOperationException("Las unidades de medida son obligatorias.");
                                }
                            }

                            // Crear el insumo principal
                            var insumo = new Insumo
                            {
                                Codigo = viewModel.Codigo,
                                Nombre = viewModel.Nombre,
                                Tipo = viewModel.Tipo,
                                Descripcion = viewModel.Descripcion,
                                RangoDosis = GenerarResumenRangos(viewModel.ConfiguracionesRango),
                                Estado = viewModel.Estado,
                                FechaCreacion = DateTime.UtcNow,
                                EmpresaId = CurrentEmpresaId
                            };

                            _context.Add(insumo);
                            await _context.SaveChangesAsync(); // Guardar para obtener el ID del insumo

                            // Crear las configuraciones de rango
                            foreach (var configViewModel in viewModel.ConfiguracionesRango)
                            {
                                var configuracionRango = new ConfiguracionRangoInsumo
                                {
                                    InsumoId = insumo.Id,
                                    EdadMinima = configViewModel.EdadMinima,
                                    EdadMaxima = configViewModel.EdadMaxima,
                                    UnidadMedidaEdadMinima = configViewModel.UnidadMedidaEdadMinima,
                                    UnidadMedidaEdadMaxima = configViewModel.UnidadMedidaEdadMaxima,
                                    Dosis = configViewModel.Dosis,
                                    DescripcionRango = configViewModel.DescripcionRango,
                                    FechaCreacion = DateTime.UtcNow,
                                    Estado = true
                                };

                                _context.ConfiguracionesRangoInsumo.Add(configuracionRango);
                            }

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    });

                    TempData["Success"] = $"Insumo creado exitosamente con {viewModel.ConfiguracionesRango.Count} configuración(es) de rango.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el insumo: " + ex.Message);
                }
            }

            return View(viewModel);
        }

        // GET: Insumo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos
                .Include(i => i.ConfiguracionesRango.Where(cr => cr.Estado))
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (insumo == null)
            {
                return NotFound();
            }

            var viewModel = new InsumoEditViewModel
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                Tipo = insumo.Tipo,
                Descripcion = insumo.Descripcion,
                Estado = insumo.Estado,
                ConfiguracionesRango = insumo.ConfiguracionesRango.Select(cr => new ConfiguracionRangoInsumoViewModel
                {
                    Id = cr.Id,
                    EdadMinima = cr.EdadMinima,
                    EdadMaxima = cr.EdadMaxima,
                    UnidadMedidaEdadMinima = cr.UnidadMedidaEdadMinima,
                    UnidadMedidaEdadMaxima = cr.UnidadMedidaEdadMaxima,
                    Dosis = cr.Dosis,
                    DescripcionRango = cr.DescripcionRango,
                    Estado = cr.Estado
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Insumo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, InsumoEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Usar la estrategia de ejecución de Entity Framework para manejar transacciones
                    var strategy = _context.Database.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await _context.Database.BeginTransactionAsync();

                        try
                        {
                            // Verificar si el código ya existe en otro insumo
                            var existeCodigo = await _context.Insumos
                                .AnyAsync(i => i.Codigo == viewModel.Codigo && i.Id != id && i.EmpresaId == CurrentEmpresaId);

                            if (existeCodigo)
                            {
                                throw new InvalidOperationException("Ya existe otro insumo con este código.");
                            }

                            var insumo = await _context.Insumos
                                .Include(i => i.ConfiguracionesRango)
                                .FirstOrDefaultAsync(i => i.Id == id && i.EmpresaId == CurrentEmpresaId);

                            if (insumo == null)
                            {
                                throw new InvalidOperationException("Insumo no encontrado.");
                            }

                            // Actualizar propiedades del insumo
                            insumo.Codigo = viewModel.Codigo;
                            insumo.Nombre = viewModel.Nombre;
                            insumo.Tipo = viewModel.Tipo;
                            insumo.Descripcion = viewModel.Descripcion;
                            insumo.Estado = viewModel.Estado; 
                            
                            // Agregar nuevas configuraciones
                            if (viewModel.ConfiguracionesRango != null && viewModel.ConfiguracionesRango.Any())
                            {
                                foreach (var configViewModel in viewModel.ConfiguracionesRango)
                                {
                                    var configuracionRango = new ConfiguracionRangoInsumo
                                    {
                                        InsumoId = insumo.Id,
                                        EdadMinima = configViewModel.EdadMinima,
                                        EdadMaxima = configViewModel.EdadMaxima,
                                        UnidadMedidaEdadMinima = configViewModel.UnidadMedidaEdadMinima,
                                        UnidadMedidaEdadMaxima = configViewModel.UnidadMedidaEdadMaxima,
                                        Dosis = configViewModel.Dosis,
                                        DescripcionRango = configViewModel.DescripcionRango,
                                        FechaCreacion = DateTime.UtcNow,
                                        Estado = true
                                    };

                                    insumo.ConfiguracionesRango.Add(configuracionRango);
                                }
                            }

                            _context.Update(insumo);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        catch
                        {
                            await transaction.RollbackAsync();
                            throw;
                        }
                    });

                    TempData["Success"] = "Insumo actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InsumoExists(viewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el insumo: " + ex.Message);
                }
            }

            return View(viewModel);
        }
        // Método auxiliar para generar resumen de rangos
        private string GenerarResumenRangos(List<ConfiguracionRangoInsumoViewModel> configuraciones)
        {
            if (configuraciones == null || !configuraciones.Any())
                return "Sin configuraciones";

            var resumenes = configuraciones.Select(c => c.DescripcionRango).Where(d => !string.IsNullOrEmpty(d));
            return string.Join(" | ", resumenes);
        }

        // GET: Insumo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos
                .Include(i => i.ConfiguracionesRango.Where(cr => cr.Estado))
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (insumo == null)
            {
                return NotFound();
            }

            var viewModel = new InsumoViewModel
            {
                Id = insumo.Id,
                Codigo = insumo.Codigo,
                Nombre = insumo.Nombre,
                Tipo = insumo.Tipo,
                Descripcion = insumo.Descripcion,
                RangoDosis = insumo.RangoDosis,
                Estado = insumo.Estado,
                FechaCreacion = insumo.FechaCreacion,
                ConfiguracionesRango = insumo.ConfiguracionesRango.Select(cr => new ConfiguracionRangoInsumoViewModel
                {
                    Id = cr.Id,
                    EdadMinima = cr.EdadMinima,
                    EdadMaxima = cr.EdadMaxima,
                    UnidadMedidaEdadMinima = cr.UnidadMedidaEdadMinima,
                    UnidadMedidaEdadMaxima = cr.UnidadMedidaEdadMaxima,
                    Dosis = cr.Dosis,
                    DescripcionRango = cr.DescripcionRango,
                    Estado = cr.Estado
                }).ToList()
            };

            return View(viewModel);
        }

        // Método auxiliar para verificar si existe el insumo
        private bool InsumoExists(int id)
        {
            return _context.Insumos.Any(e => e.Id == id);
        }

        // AJAX Methods for modal operations
        [HttpGet]
        public async Task<JsonResult> GetById(int id)
        {
            try
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (insumo == null)
                {
                    return Json(new { success = false, message = "Insumo no encontrado" });
                }

                var viewModel = new InsumoItemViewModel
                {
                    Id = insumo.Id,
                    Codigo = insumo.Codigo,
                    Nombre = insumo.Nombre,
                    Tipo = insumo.Tipo,
                    Descripcion = insumo.Descripcion,
                    RangoDosis = insumo.RangoDosis,
                    Estado = insumo.Estado,
                    FechaCreacion = insumo.FechaCreacion
                };

                return Json(new { success = true, data = viewModel });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateAjax([FromBody] InsumoCreateViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                        .ToArray();
                    return Json(new { success = false, message = "Datos inválidos", errors });
                }

                // Verificar si el código ya existe
                var existeCodigo = await _context.Insumos
                    .AnyAsync(i => i.Codigo == viewModel.Codigo && i.EmpresaId == CurrentEmpresaId);

                if (existeCodigo)
                {
                    return Json(new { success = false, message = "Ya existe un insumo con este código." });
                }

                var insumo = new Insumo
                {
                    Codigo = viewModel.Codigo,
                    Nombre = viewModel.Nombre,
                    Tipo = viewModel.Tipo,
                    Descripcion = viewModel.Descripcion,
                    Estado = viewModel.Estado,
                    FechaCreacion = DateTime.UtcNow
                };

                _context.Add(insumo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Insumo creado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EditAjax([FromBody] InsumoEditViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Message = x.Value.Errors.First().ErrorMessage })
                        .ToArray();
                    return Json(new { success = false, message = "Datos inválidos", errors });
                }

                // Verificar si el código ya existe en otro registro
                var existeCodigo = await _context.Insumos
                    .AnyAsync(i => i.Codigo == viewModel.Codigo && i.Id != viewModel.Id && i.EmpresaId == CurrentEmpresaId);

                if (existeCodigo)
                {
                    return Json(new { success = false, message = "Ya existe otro insumo con este código." });
                }

                var insumo = await _context.Insumos.FindAsync(viewModel.Id);
                if (insumo == null)
                {
                    return Json(new { success = false, message = "Insumo no encontrado" });
                }

                insumo.Codigo = viewModel.Codigo;
                insumo.Nombre = viewModel.Nombre;
                insumo.Tipo = viewModel.Tipo;
                insumo.Descripcion = viewModel.Descripcion;
                insumo.RangoDosis = viewModel.RangoDosis;
                insumo.Estado = viewModel.Estado;

                _context.Update(insumo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Insumo actualizado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                // Usar la estrategia de ejecución de Entity Framework para manejar transacciones
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        var insumo = await _context.Insumos
                            .Include(i => i.ConfiguracionesRango)
                            .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

                        if (insumo == null)
                        {
                            throw new InvalidOperationException("Insumo no encontrado");
                        }

                        // Eliminar primero las configuraciones de rango asociadas
                        if (insumo.ConfiguracionesRango != null && insumo.ConfiguracionesRango.Any())
                        {
                            _context.ConfiguracionesRangoInsumo.RemoveRange(insumo.ConfiguracionesRango);
                        }

                        // Luego eliminar el insumo
                        _context.Insumos.Remove(insumo);

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });

                TempData["Success"] = "Insumo y sus configuraciones eliminados exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el insumo: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<JsonResult> AgregarConfiguracionRango([FromBody] AgregarConfiguracionRangoRequest request)
        {
            try
            {
                // 1. VALIDACIONES DE ENTRADA MÁS ROBUSTAS
                if (request == null)
                {
                    return Json(new { success = false, message = "Los datos de la configuración son requeridos" });
                }

                if (request.InsumoId <= 0)
                {
                    return Json(new { success = false, message = "El ID del insumo es inválido" });
                }

                if (string.IsNullOrWhiteSpace(request.UnidadMedidaEdadMinima) ||
                    string.IsNullOrWhiteSpace(request.UnidadMedidaEdadMaxima))
                {
                    return Json(new { success = false, message = "Las unidades de medida son obligatorias" });
                }

                if (string.IsNullOrWhiteSpace(request.DescripcionRango))
                {
                    return Json(new { success = false, message = "La descripción del rango es obligatoria" });
                }

                // 2. USAR LA ESTRATEGIA DE EJECUCIÓN DE ENTITY FRAMEWORK CORRECTAMENTE
                var strategy = _context.Database.CreateExecutionStrategy();

                // Crear una clase para el resultado de la operación
                var result = await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        // Validar que el insumo existe y pertenece a la empresa actual
                        var insumo = await _context.Insumos
                            .FirstOrDefaultAsync(i => i.Id == request.InsumoId && i.EmpresaId == CurrentEmpresaId);

                        if (insumo == null)
                        {
                            throw new InvalidOperationException("Insumo no encontrado o no tiene permisos para modificarlo");
                        }

                        // 3. VALIDAR RANGOS DE EDAD CON MEJOR MANEJO DE UNIDADES
                        var edadMinEnDias = ConvertirADias(request.EdadMinima, request.UnidadMedidaEdadMinima);
                        var edadMaxEnDias = ConvertirADias(request.EdadMaxima, request.UnidadMedidaEdadMaxima);

                        if (edadMinEnDias <= 0 || edadMaxEnDias <= 0)
                        {
                            throw new InvalidOperationException("Las unidades de medida especificadas no son válidas");
                        }

                        if (edadMinEnDias >= edadMaxEnDias)
                        {
                            throw new InvalidOperationException("La edad máxima debe ser mayor que la edad mínima considerando las unidades de medida");
                        }


                        // 5. CREAR LA NUEVA CONFIGURACIÓN
                        var configuracionRango = new ConfiguracionRangoInsumo
                        {
                            InsumoId = request.InsumoId,
                            EdadMinima = request.EdadMinima,
                            EdadMaxima = request.EdadMaxima,
                            UnidadMedidaEdadMinima = request.UnidadMedidaEdadMinima.Trim(),
                            UnidadMedidaEdadMaxima = request.UnidadMedidaEdadMaxima.Trim(),
                            Dosis = string.IsNullOrWhiteSpace(request.Dosis) ? null : request.Dosis.Trim(),
                            DescripcionRango = request.DescripcionRango.Trim(),
                            FechaCreacion = DateTime.UtcNow,
                            Estado = true
                        };

                        _context.ConfiguracionesRangoInsumo.Add(configuracionRango);
                        await _context.SaveChangesAsync();

                        // 6. ACTUALIZAR EL RESUMEN DE RANGOS DEL INSUMO
                        // Primero obtener todas las configuraciones sin ordenar
                        var todasLasConfiguracionesRaw = await _context.ConfiguracionesRangoInsumo
                            .Where(cr => cr.InsumoId == request.InsumoId && cr.Estado)
                            .Select(cr => new ConfiguracionRangoInsumoViewModel
                            {
                                Id = cr.Id,
                                EdadMinima = cr.EdadMinima,
                                EdadMaxima = cr.EdadMaxima,
                                UnidadMedidaEdadMinima = cr.UnidadMedidaEdadMinima,
                                UnidadMedidaEdadMaxima = cr.UnidadMedidaEdadMaxima,
                                Dosis = cr.Dosis,
                                DescripcionRango = cr.DescripcionRango
                            })
                            .ToListAsync();

                        // Ahora ordenar en memoria usando ConvertirADias
                        var todasLasConfiguraciones = todasLasConfiguracionesRaw
                            .OrderBy(cr => ConvertirADias(cr.EdadMinima, cr.UnidadMedidaEdadMinima))
                            .ToList();

                        insumo.RangoDosis = GenerarResumenRangos(todasLasConfiguraciones);
                        _context.Update(insumo);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        // Retornar la configuración creada para usar fuera del ExecuteAsync
                        return configuracionRango;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error en transacción interna AgregarConfiguracionRango: {ex.Message}");
                        throw;
                    }
                });

                // Construir la respuesta después de que la transacción sea exitosa
                return Json(new
                {
                    success = true,
                    message = "Configuración agregada exitosamente",
                    configuracion = new
                    {
                        Id = result.Id,
                        EdadMinima = result.EdadMinima,
                        EdadMaxima = result.EdadMaxima,
                        UnidadMedidaEdadMinima = result.UnidadMedidaEdadMinima,
                        UnidadMedidaEdadMaxima = result.UnidadMedidaEdadMaxima,
                        Dosis = result.Dosis,
                        DescripcionRango = result.DescripcionRango,
                        FechaCreacion = result.FechaCreacion
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al agregar configuración de rango: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");

                return Json(new
                {
                    success = false,
                    message = "Error interno del servidor: " + ex.Message,
                    details = ex.InnerException?.Message ?? "Sin detalles adicionales"
                });
            }
        }

        [HttpPost]
        public async Task<JsonResult> EliminarConfiguracionRango(int id)
        {
            try
            {
                // Usar la estrategia de ejecución de Entity Framework
                var strategy = _context.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        var configuracion = await _context.ConfiguracionesRangoInsumo
                            .Include(cr => cr.Insumo)
                            .FirstOrDefaultAsync(cr => cr.Id == id && cr.Insumo.EmpresaId == CurrentEmpresaId);

                        if (configuracion == null)
                        {
                            throw new InvalidOperationException("Configuración no encontrada");
                        }

                        var insumoId = configuracion.InsumoId;
                        _context.ConfiguracionesRangoInsumo.Remove(configuracion);
                        await _context.SaveChangesAsync();

                        // Actualizar el resumen de rangos del insumo
                        var insumo = await _context.Insumos.FindAsync(insumoId);
                        if (insumo != null)
                        {
                            var configuracionesRestantes = await _context.ConfiguracionesRangoInsumo
                                .Where(cr => cr.InsumoId == insumoId && cr.Estado)
                                .Select(cr => new ConfiguracionRangoInsumoViewModel
                                {
                                    Id = cr.Id,
                                    EdadMinima = cr.EdadMinima,
                                    EdadMaxima = cr.EdadMaxima,
                                    UnidadMedidaEdadMinima = cr.UnidadMedidaEdadMinima,
                                    UnidadMedidaEdadMaxima = cr.UnidadMedidaEdadMaxima,
                                    Dosis = cr.Dosis,
                                    DescripcionRango = cr.DescripcionRango
                                })
                                .ToListAsync();

                            insumo.RangoDosis = GenerarResumenRangos(configuracionesRestantes);
                            _context.Update(insumo);
                            await _context.SaveChangesAsync();
                        }

                        await transaction.CommitAsync();

                        // Retornar un valor simple para el ExecuteAsync
                        return true;
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        Console.WriteLine($"Error en transacción EliminarConfiguracionRango: {ex.Message}");
                        throw;
                    }
                });

                return Json(new { success = true, message = "Configuración eliminada exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar configuración de rango: {ex.Message}");
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        public class AgregarConfiguracionRangoRequest
        {
            public int InsumoId { get; set; }
            public int EdadMinima { get; set; }
            public int EdadMaxima { get; set; }
            public string UnidadMedidaEdadMinima { get; set; } = string.Empty;
            public string UnidadMedidaEdadMaxima { get; set; } = string.Empty;
            public string? Dosis { get; set; }
            public string DescripcionRango { get; set; } = string.Empty;
        }
        private int ConvertirADias(int edad, string unidad)
        {
            if (string.IsNullOrWhiteSpace(unidad))
                return 0;

            return unidad.Trim().ToLowerInvariant() switch
            {
                "dias" or "días" or "day" or "days" => edad,
                "meses" or "mes" or "months" or "month" => edad * 30,  // Aproximadamente 30 días por mes
                "anos" or "años" or "año" or "years" or "year" => edad * 365,  // Aproximadamente 365 días por año
                _ => 0
            };
        }

        // AGREGAR MÉTODO PARA VALIDAR LA ESTRUCTURA DE LA REQUEST
        private bool ValidarRequest(AgregarConfiguracionRangoRequest request, out string mensajeError)
        {
            mensajeError = string.Empty;

            if (request == null)
            {
                mensajeError = "Los datos de la configuración son requeridos";
                return false;
            }

            if (request.InsumoId <= 0)
            {
                mensajeError = "El ID del insumo es inválido";
                return false;
            }

            if (request.EdadMinima < 0 || request.EdadMaxima < 0)
            {
                mensajeError = "Las edades no pueden ser negativas";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.UnidadMedidaEdadMinima) ||
                string.IsNullOrWhiteSpace(request.UnidadMedidaEdadMaxima))
            {
                mensajeError = "Las unidades de medida son obligatorias";
                return false;
            }

            if (string.IsNullOrWhiteSpace(request.DescripcionRango))
            {
                mensajeError = "La descripción del rango es obligatoria";
                return false;
            }

            return true;
        }
    }
}