using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;

namespace Highdmin.Controllers
{
    public class RegimenAfiliacionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RegimenAfiliacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RegimenAfiliacion
        public async Task<IActionResult> Index()
        {
            try
            {
                var regimenesAfiliacion = await _context.RegimenesAfiliacion
                    .OrderBy(r => r.Codigo)
                    .Select(r => new RegimenAfiliacionItemViewModel
                    {
                        Id = r.Id,
                        Codigo = r.Codigo,
                        Nombre = r.Nombre,
                        Descripcion = r.Descripcion,
                        Estado = r.Estado,
                        FechaCreacion = r.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new RegimenAfiliacionViewModel
                {
                    TotalRegimenes = regimenesAfiliacion.Count,
                    RegimenesActivos = regimenesAfiliacion.Count(r => r.Estado),
                    RegimenesInactivos = regimenesAfiliacion.Count(r => !r.Estado),
                    RegimenesAfiliacion = regimenesAfiliacion
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los regímenes de afiliación: " + ex.Message;
                return View(new RegimenAfiliacionViewModel());
            }
        }

        // GET: RegimenAfiliacion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimenAfiliacion = await _context.RegimenesAfiliacion
                .FirstOrDefaultAsync(m => m.Id == id);

            if (regimenAfiliacion == null)
            {
                return NotFound();
            }

            var viewModel = new RegimenAfiliacionItemViewModel
            {
                Id = regimenAfiliacion.Id,
                Codigo = regimenAfiliacion.Codigo,
                Nombre = regimenAfiliacion.Nombre,
                Descripcion = regimenAfiliacion.Descripcion,
                Estado = regimenAfiliacion.Estado,
                FechaCreacion = regimenAfiliacion.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: RegimenAfiliacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RegimenAfiliacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegimenAfiliacionCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.RegimenesAfiliacion
                        .AnyAsync(r => r.Codigo == viewModel.Codigo);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un régimen de afiliación con este código.");
                        return View(viewModel);
                    }

                    var regimenAfiliacion = new RegimenAfiliacion
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Add(regimenAfiliacion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Régimen de afiliación creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear el régimen de afiliación: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: RegimenAfiliacion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimenAfiliacion = await _context.RegimenesAfiliacion.FindAsync(id);
            if (regimenAfiliacion == null)
            {
                return NotFound();
            }

            var viewModel = new RegimenAfiliacionEditViewModel
            {
                Id = regimenAfiliacion.Id,
                Codigo = regimenAfiliacion.Codigo,
                Nombre = regimenAfiliacion.Nombre,
                Descripcion = regimenAfiliacion.Descripcion,
                Estado = regimenAfiliacion.Estado,
                FechaCreacion = regimenAfiliacion.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: RegimenAfiliacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RegimenAfiliacionEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe (excluyendo el registro actual)
                    var existeCodigo = await _context.RegimenesAfiliacion
                        .AnyAsync(r => r.Codigo == viewModel.Codigo && r.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un régimen de afiliación con este código.");
                        return View(viewModel);
                    }

                    var regimenAfiliacion = await _context.RegimenesAfiliacion.FindAsync(id);
                    if (regimenAfiliacion == null)
                    {
                        return NotFound();
                    }

                    regimenAfiliacion.Codigo = viewModel.Codigo;
                    regimenAfiliacion.Nombre = viewModel.Nombre;
                    regimenAfiliacion.Descripcion = viewModel.Descripcion;
                    regimenAfiliacion.Estado = viewModel.Estado;

                    _context.Update(regimenAfiliacion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Régimen de afiliación actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RegimenAfiliacionExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar el régimen de afiliación: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: RegimenAfiliacion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var regimenAfiliacion = await _context.RegimenesAfiliacion
                .FirstOrDefaultAsync(m => m.Id == id);

            if (regimenAfiliacion == null)
            {
                return NotFound();
            }

            var viewModel = new RegimenAfiliacionItemViewModel
            {
                Id = regimenAfiliacion.Id,
                Codigo = regimenAfiliacion.Codigo,
                Nombre = regimenAfiliacion.Nombre,
                Descripcion = regimenAfiliacion.Descripcion,
                Estado = regimenAfiliacion.Estado,
                FechaCreacion = regimenAfiliacion.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: RegimenAfiliacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var regimenAfiliacion = await _context.RegimenesAfiliacion.FindAsync(id);
                if (regimenAfiliacion != null)
                {
                    _context.RegimenesAfiliacion.Remove(regimenAfiliacion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Régimen de afiliación eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el régimen de afiliación: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: RegimenAfiliacion/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var regimenAfiliacion = await _context.RegimenesAfiliacion.FindAsync(id);
                if (regimenAfiliacion != null)
                {
                    regimenAfiliacion.Estado = !regimenAfiliacion.Estado;
                    _context.Update(regimenAfiliacion);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = regimenAfiliacion.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Régimen de afiliación no encontrado" });
        }

        private bool RegimenAfiliacionExists(int id)
        {
            return _context.RegimenesAfiliacion.Any(e => e.Id == id);
        }
    }
}