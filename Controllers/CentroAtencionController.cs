using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class CentroAtencionController : BaseEmpresaController
    {
        private readonly ApplicationDbContext _context; 

        public CentroAtencionController(ApplicationDbContext context, IEmpresaService empresaService): base(empresaService)
        {
            _context = context;
        }

        // GET: CentroAtencion
        public async Task<IActionResult> Index()
        {
            try
            { 
                var centrosAtencion = await _context.CentrosAtencion
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(c => c.Codigo)
                    .Select(c => new CentroAtencionItemViewModel
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        Nombre = c.Nombre,
                        Tipo = c.Tipo,
                        Estado = c.Estado,
                        Descripcion = c.Descripcion,
                        FechaCreacion = c.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new CentroAtencionViewModel
                {
                    TotalCentros = centrosAtencion.Count,
                    CentrosActivos = centrosAtencion.Count(c => c.Estado),
                    CentrosInactivos = centrosAtencion.Count(c => !c.Estado),
                    CentrosAtencion = centrosAtencion
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los centros de atención: " + ex.Message;
                return View(new CentroAtencionViewModel());
            }
        }

        // GET: CentroAtencion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centroAtencion = await _context.CentrosAtencion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (centroAtencion == null)
            {
                return NotFound();
            }

            var viewModel = new CentroAtencionItemViewModel
            {
                Id = centroAtencion.Id,
                Codigo = centroAtencion.Codigo,
                Nombre = centroAtencion.Nombre,
                Tipo = centroAtencion.Tipo,
                Estado = centroAtencion.Estado,
                Descripcion = centroAtencion.Descripcion,
                FechaCreacion = centroAtencion.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: CentroAtencion/Create
        public IActionResult Create()
        {
            return View(new CentroAtencionCreateViewModel());
        }

        // POST: CentroAtencion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CentroAtencionCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.CentrosAtencion
                        .AnyAsync(c => c.Codigo == viewModel.Codigo && c.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un centro de atención con este código.");
                        return View(viewModel);
                    }

                    var centroAtencion = new CentroAtencion
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Tipo = viewModel.Tipo,
                        Estado = viewModel.Estado,
                        Descripcion = viewModel.Descripcion,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(centroAtencion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Centro de atención creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear el centro de atención: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CentroAtencion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centroAtencion = await _context.CentrosAtencion
                .FirstOrDefaultAsync(c => c.Id == id && c.EmpresaId == CurrentEmpresaId);
            if (centroAtencion == null)
            {
                return NotFound();
            }

            var viewModel = new CentroAtencionEditViewModel
            {
                Id = centroAtencion.Id,
                Codigo = centroAtencion.Codigo,
                Nombre = centroAtencion.Nombre,
                Tipo = centroAtencion.Tipo,
                Estado = centroAtencion.Estado,
                FechaCreacion = centroAtencion.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: CentroAtencion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CentroAtencionEditViewModel viewModel)
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
                    var existeCodigo = await _context.CentrosAtencion
                        .AnyAsync(c => c.Codigo == viewModel.Codigo && c.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un centro de atención con este código.");
                        return View(viewModel);
                    }

                    var centroAtencion = await _context.CentrosAtencion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (centroAtencion == null)
                    {
                        return NotFound();
                    }

                    centroAtencion.Codigo = viewModel.Codigo;
                    centroAtencion.Nombre = viewModel.Nombre;
                    centroAtencion.Tipo = viewModel.Tipo;
                    centroAtencion.Estado = viewModel.Estado;
                    centroAtencion.Descripcion = viewModel.Descripcion;

                    _context.Update(centroAtencion);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Centro de atención actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CentroAtencionExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar el centro de atención: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CentroAtencion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var centroAtencion = await _context.CentrosAtencion
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (centroAtencion == null)
            {
                return NotFound();
            }

            var viewModel = new CentroAtencionItemViewModel
            {
                Id = centroAtencion.Id,
                Codigo = centroAtencion.Codigo,
                Nombre = centroAtencion.Nombre,
                Tipo = centroAtencion.Tipo,
                Estado = centroAtencion.Estado,
                FechaCreacion = centroAtencion.FechaCreacion,
                Descripcion = centroAtencion.Descripcion
            };

            return View(viewModel);
        }

        // POST: CentroAtencion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var centroAtencion = await _context.CentrosAtencion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (centroAtencion != null)
                {
                    _context.CentrosAtencion.Remove(centroAtencion);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Centro de atención eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el centro de atención: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: CentroAtencion/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var centroAtencion = await _context.CentrosAtencion.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (centroAtencion != null)
                {
                    centroAtencion.Estado = !centroAtencion.Estado;
                    _context.Update(centroAtencion);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = centroAtencion.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Centro de atención no encontrado" });
        }

        private bool CentroAtencionExists(int id)
        {
            return _context.CentrosAtencion.Any(e => e.Id == id);
        }
    }
}