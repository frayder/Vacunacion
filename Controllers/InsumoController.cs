using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class InsumoController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public InsumoController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService, authorizationService)
        {
            _context = context;
        }

        // GET: Insumo
        public async Task<IActionResult> Index()
        {
            try
            {
                // Validar permisos y obtener todos los permisos del módulo
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
                        Tipo = i.Tipo,
                        Descripcion = i.Descripcion,
                        RangoDosis = i.RangoDosis,
                        Estado = i.Estado,
                        FechaCreacion = i.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new InsumoViewModel
                {
                    TotalInsumos = insumos.Count,
                    InsumosActivos = insumos.Count(i => i.Estado),
                    InsumosInactivos = insumos.Count(i => !i.Estado),
                    Insumos = insumos,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los insumos: " + ex.Message;
                return View(new InsumoViewModel());
            }
        }

        // GET: Insumo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (insumo == null)
            {
                return NotFound();
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

            return View(viewModel);
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
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.Insumos
                        .AnyAsync(i => i.Codigo == viewModel.Codigo && i.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un insumo con este código.");
                        return View(viewModel);
                    }

                    // Construir rango/dosis si se proporcionaron datos
                    string? rangoDosis = null;
                    if (!string.IsNullOrEmpty(viewModel.DescripcionRango) || 
                        viewModel.EdadMinimaDias.HasValue || 
                        viewModel.EdadMaximaDias.HasValue || 
                        !string.IsNullOrEmpty(viewModel.Dosis))
                    {
                        var rangoPartes = new List<string>();
                        
                        if (!string.IsNullOrEmpty(viewModel.DescripcionRango))
                            rangoPartes.Add(viewModel.DescripcionRango);
                            
                        if (viewModel.EdadMinimaDias.HasValue && viewModel.EdadMaximaDias.HasValue)
                            rangoPartes.Add($"{viewModel.EdadMinimaDias}-{viewModel.EdadMaximaDias} días");
                        else if (viewModel.EdadMinimaDias.HasValue)
                            rangoPartes.Add($"Desde {viewModel.EdadMinimaDias} días");
                        else if (viewModel.EdadMaximaDias.HasValue)
                            rangoPartes.Add($"Hasta {viewModel.EdadMaximaDias} días");
                            
                        if (!string.IsNullOrEmpty(viewModel.Dosis))
                            rangoPartes.Add(viewModel.Dosis);
                            
                        rangoDosis = string.Join(" - ", rangoPartes);
                    }

                    var insumo = new Insumo
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Tipo = viewModel.Tipo,
                        Descripcion = viewModel.Descripcion,
                        RangoDosis = rangoDosis,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(insumo);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Insumo creado exitosamente.";
                    return RedirectToAction(nameof(Index));
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

            var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
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
                RangoDosis = insumo.RangoDosis,
                Estado = insumo.Estado,
                FechaCreacion = insumo.FechaCreacion
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
                    // Verificar si el código ya existe en otro registro
                    var existeCodigo = await _context.Insumos
                        .AnyAsync(i => i.Codigo == viewModel.Codigo && i.Id != id && i.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe otro insumo con este código.");
                        return View(viewModel);
                    }

                    var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (insumo == null)
                    {
                        return NotFound();
                    }

                    insumo.Codigo = viewModel.Codigo;
                    insumo.Nombre = viewModel.Nombre;
                    insumo.Tipo = viewModel.Tipo;
                    insumo.Descripcion = viewModel.Descripcion;
                    insumo.RangoDosis = viewModel.RangoDosis;
                    insumo.Estado = viewModel.Estado;

                    _context.Update(insumo);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Insumo actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
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

        // GET: Insumo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var insumo = await _context.Insumos
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (insumo == null)
            {
                return NotFound();
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

            return View(viewModel);
        }

        // POST: Insumo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (insumo != null)
                {
                    _context.Insumos.Remove(insumo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Insumo eliminado exitosamente.";
                }
                else
                {
                    TempData["Error"] = "No se encontró el insumo a eliminar.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el insumo: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

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
                    FechaCreacion = DateTime.Now
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
        public async Task<JsonResult> DeleteAjax(int id)
        {
            try
            {
                var insumo = await _context.Insumos.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (insumo == null)
                {
                    return Json(new { success = false, message = "Insumo no encontrado" });
                }

                _context.Insumos.Remove(insumo);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Insumo eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }
    }
}