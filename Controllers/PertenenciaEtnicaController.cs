using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class PertenenciaEtnicaController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public PertenenciaEtnicaController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService, authorizationService)
        {
            _context = context;
        }

        // GET: PertenenciaEtnica
        public async Task<IActionResult> Index()
        {
            try
            {
                // Validar permisos y obtener todos los permisos del módulo
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("PertenenciaEtnica", "Read");
                if (redirect != null) return redirect;

                var pertenenciasEtnicas = await _context.PertenenciasEtnicas
                    .Where(p => p.EmpresaId == CurrentEmpresaId)
                    .OrderBy(p => p.Codigo)
                    .Select(p => new PertenenciaEtnicaItemViewModel
                    {
                        Id = p.Id,
                        Codigo = p.Codigo,
                        Nombre = p.Nombre,
                        Descripcion = p.Descripcion,
                        Estado = p.Estado,
                        FechaCreacion = p.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new PertenenciaEtnicaViewModel
                {
                    TotalPertenencias = pertenenciasEtnicas.Count,
                    PertenenciasActivas = pertenenciasEtnicas.Count(p => p.Estado),
                    PertenenciasInactivas = pertenenciasEtnicas.Count(p => !p.Estado),
                    PertenenciasEtnicas = pertenenciasEtnicas,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las pertenencias étnicas: " + ex.Message;
                return View(new PertenenciaEtnicaViewModel());
            }
        }

        // GET: PertenenciaEtnica/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pertenenciaEtnica = await _context.PertenenciasEtnicas
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (pertenenciaEtnica == null)
            {
                return NotFound();
            }

            var viewModel = new PertenenciaEtnicaItemViewModel
            {
                Id = pertenenciaEtnica.Id,
                Codigo = pertenenciaEtnica.Codigo,
                Nombre = pertenenciaEtnica.Nombre,
                Descripcion = pertenenciaEtnica.Descripcion,
                Estado = pertenenciaEtnica.Estado,
                FechaCreacion = pertenenciaEtnica.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: PertenenciaEtnica/Create
        public IActionResult Create()
        {
            return View(new PertenenciaEtnicaCreateViewModel());
        }

        // POST: PertenenciaEtnica/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PertenenciaEtnicaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.PertenenciasEtnicas
                        .AnyAsync(p => p.Codigo == viewModel.Codigo && p.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una pertenencia étnica con este código.");
                        return View(viewModel);
                    }

                    var pertenenciaEtnica = new PertenenciaEtnica
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(pertenenciaEtnica);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Pertenencia étnica creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la pertenencia étnica: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: PertenenciaEtnica/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pertenenciaEtnica = await _context.PertenenciasEtnicas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (pertenenciaEtnica == null)
            {
                return NotFound();
            }

            var viewModel = new PertenenciaEtnicaEditViewModel
            {
                Id = pertenenciaEtnica.Id,
                Codigo = pertenenciaEtnica.Codigo,
                Nombre = pertenenciaEtnica.Nombre,
                Descripcion = pertenenciaEtnica.Descripcion,
                Estado = pertenenciaEtnica.Estado,
                FechaCreacion = pertenenciaEtnica.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: PertenenciaEtnica/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PertenenciaEtnicaEditViewModel viewModel)
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
                    var existeCodigo = await _context.PertenenciasEtnicas
                        .AnyAsync(p => p.Codigo == viewModel.Codigo && p.Id != id && p.EmpresaId == CurrentEmpresaId);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una pertenencia étnica con este código.");
                        return View(viewModel);
                    }

                    var pertenenciaEtnica = await _context.PertenenciasEtnicas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (pertenenciaEtnica == null)
                    {
                        return NotFound();
                    }

                    pertenenciaEtnica.Codigo = viewModel.Codigo;
                    pertenenciaEtnica.Nombre = viewModel.Nombre;
                    pertenenciaEtnica.Descripcion = viewModel.Descripcion;
                    pertenenciaEtnica.Estado = viewModel.Estado;

                    _context.Update(pertenenciaEtnica);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Pertenencia étnica actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PertenenciaEtnicaExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar la pertenencia étnica: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: PertenenciaEtnica/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pertenenciaEtnica = await _context.PertenenciasEtnicas
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (pertenenciaEtnica == null)
            {
                return NotFound();
            }

            var viewModel = new PertenenciaEtnicaItemViewModel
            {
                Id = pertenenciaEtnica.Id,
                Codigo = pertenenciaEtnica.Codigo,
                Nombre = pertenenciaEtnica.Nombre,
                Descripcion = pertenenciaEtnica.Descripcion,
                Estado = pertenenciaEtnica.Estado,
                FechaCreacion = pertenenciaEtnica.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: PertenenciaEtnica/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var pertenenciaEtnica = await _context.PertenenciasEtnicas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (pertenenciaEtnica != null)
                {
                    _context.PertenenciasEtnicas.Remove(pertenenciaEtnica);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Pertenencia étnica eliminada exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la pertenencia étnica: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: PertenenciaEtnica/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var pertenenciaEtnica = await _context.PertenenciasEtnicas
                    .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (pertenenciaEtnica != null)
                {
                    pertenenciaEtnica.Estado = !pertenenciaEtnica.Estado;
                    _context.Update(pertenenciaEtnica);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = pertenenciaEtnica.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Pertenencia étnica no encontrada" });
        }

        private bool PertenenciaEtnicaExists(int id)
        {
            return _context.PertenenciasEtnicas.Any(e => e.Id == id && e.EmpresaId == CurrentEmpresaId);
        }
    }
}