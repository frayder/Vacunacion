using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class AseguradoraController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public AseguradoraController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService, authorizationService)
        {
            _context = context;
        }

        // GET: Aseguradora
        public async Task<IActionResult> Index()
        {
            try
            {

                // Validar permisos y obtener todos los permisos del módulo
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("Aseguradora", "Read");
                if (redirect != null) return redirect;

                var aseguradoras = await _context.Aseguradoras
                    .Where(c => c.EmpresaId == CurrentEmpresaId)
                    .OrderBy(a => a.Codigo)
                    .Select(a => new AseguradoraItemViewModel
                    {
                        Id = a.Id,
                        Codigo = a.Codigo,
                        Nombre = a.Nombre,
                        Descripcion = a.Descripcion,
                        Estado = a.Estado,
                        FechaCreacion = a.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new AseguradoraViewModel
                {
                    TotalAseguradoras = aseguradoras.Count,
                    AseguradorasActivas = aseguradoras.Count(a => a.Estado),
                    AseguradorasInactivas = aseguradoras.Count(a => !a.Estado),
                    Aseguradoras = aseguradoras,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las aseguradoras: " + ex.Message;
                return View(new AseguradoraViewModel());
            }
        }

        // GET: Aseguradora/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aseguradora = await _context.Aseguradoras
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (aseguradora == null)
            {
                return NotFound();
            }

            var viewModel = new AseguradoraItemViewModel
            {
                Id = aseguradora.Id,
                Codigo = aseguradora.Codigo,
                Nombre = aseguradora.Nombre,
                Descripcion = aseguradora.Descripcion,
                Estado = aseguradora.Estado,
                FechaCreacion = aseguradora.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: Aseguradora/Create
        public IActionResult Create()
        {
            return View( new AseguradoraCreateViewModel());  
        }

        // POST: Aseguradora/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AseguradoraCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.Aseguradoras
                        .AnyAsync(a => a.Codigo == viewModel.Codigo);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una aseguradora con este código.");
                        return View(viewModel);
                    }

                    var aseguradora = new Aseguradora
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(aseguradora);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Aseguradora creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la aseguradora: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: Aseguradora/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (aseguradora == null)
            {
                return NotFound();
            }

            var viewModel = new AseguradoraEditViewModel
            {
                Id = aseguradora.Id,
                Codigo = aseguradora.Codigo,
                Nombre = aseguradora.Nombre,
                Descripcion = aseguradora.Descripcion,
                Estado = aseguradora.Estado,
                FechaCreacion = aseguradora.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: Aseguradora/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AseguradoraEditViewModel viewModel)
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
                    var existeCodigo = await _context.Aseguradoras
                        .AnyAsync(a => a.Codigo == viewModel.Codigo && a.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una aseguradora con este código.");
                        return View(viewModel);
                    }

                    var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (aseguradora == null)
                    {
                        return NotFound();
                    }

                    aseguradora.Codigo = viewModel.Codigo;
                    aseguradora.Nombre = viewModel.Nombre;
                    aseguradora.Descripcion = viewModel.Descripcion;
                    aseguradora.Estado = viewModel.Estado;

                    _context.Update(aseguradora);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Aseguradora actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AseguradoraExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar la aseguradora: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: Aseguradora/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aseguradora = await _context.Aseguradoras
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (aseguradora == null)
            {
                return NotFound();
            }

            var viewModel = new AseguradoraItemViewModel
            {
                Id = aseguradora.Id,
                Codigo = aseguradora.Codigo,
                Nombre = aseguradora.Nombre,
                Descripcion = aseguradora.Descripcion,
                Estado = aseguradora.Estado,
                FechaCreacion = aseguradora.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: Aseguradora/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (aseguradora != null)
                {
                    _context.Aseguradoras.Remove(aseguradora);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Aseguradora eliminada exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la aseguradora: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Aseguradora/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var aseguradora = await _context.Aseguradoras.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (aseguradora != null)
                {
                    aseguradora.Estado = !aseguradora.Estado;
                    _context.Update(aseguradora);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = aseguradora.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Aseguradora no encontrada" });
        }

        private bool AseguradoraExists(int id)
        {
            return _context.Aseguradoras.Any(e => e.Id == id);
        }
    }
}