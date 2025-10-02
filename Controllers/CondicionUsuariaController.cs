using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;

namespace Highdmin.Controllers
{
    public class CondicionUsuariaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CondicionUsuariaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CondicionUsuaria
        public async Task<IActionResult> Index()
        {
            try
            {
                var condicionesUsuarias = await _context.CondicionesUsuarias
                    .OrderBy(c => c.Codigo)
                    .Select(c => new CondicionUsuariaItemViewModel
                    {
                        Id = c.Id,
                        Codigo = c.Codigo,
                        Nombre = c.Nombre,
                        Descripcion = c.Descripcion,
                        Estado = c.Estado,
                        FechaCreacion = c.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new CondicionUsuariaViewModel
                {
                    TotalCondiciones = condicionesUsuarias.Count,
                    CondicionesActivas = condicionesUsuarias.Count(c => c.Estado),
                    CondicionesInactivas = condicionesUsuarias.Count(c => !c.Estado),
                    CondicionesUsuarias = condicionesUsuarias
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las condiciones usuarias: " + ex.Message;
                return View(new CondicionUsuariaViewModel());
            }
        }

        // GET: CondicionUsuaria/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionUsuaria = await _context.CondicionesUsuarias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (condicionUsuaria == null)
            {
                return NotFound();
            }

            var viewModel = new CondicionUsuariaItemViewModel
            {
                Id = condicionUsuaria.Id,
                Codigo = condicionUsuaria.Codigo,
                Nombre = condicionUsuaria.Nombre,
                Descripcion = condicionUsuaria.Descripcion,
                Estado = condicionUsuaria.Estado,
                FechaCreacion = condicionUsuaria.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: CondicionUsuaria/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CondicionUsuaria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CondicionUsuariaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.CondicionesUsuarias
                        .AnyAsync(c => c.Codigo == viewModel.Codigo);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una condición usuaria con este código.");
                        return View(viewModel);
                    }

                    var condicionUsuaria = new CondicionUsuaria
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Add(condicionUsuaria);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Condición usuaria creada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear la condición usuaria: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CondicionUsuaria/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionUsuaria = await _context.CondicionesUsuarias.FindAsync(id);
            if (condicionUsuaria == null)
            {
                return NotFound();
            }

            var viewModel = new CondicionUsuariaEditViewModel
            {
                Id = condicionUsuaria.Id,
                Codigo = condicionUsuaria.Codigo,
                Nombre = condicionUsuaria.Nombre,
                Descripcion = condicionUsuaria.Descripcion,
                Estado = condicionUsuaria.Estado,
                FechaCreacion = condicionUsuaria.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: CondicionUsuaria/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CondicionUsuariaEditViewModel viewModel)
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
                    var existeCodigo = await _context.CondicionesUsuarias
                        .AnyAsync(c => c.Codigo == viewModel.Codigo && c.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe una condición usuaria con este código.");
                        return View(viewModel);
                    }

                    var condicionUsuaria = await _context.CondicionesUsuarias.FindAsync(id);
                    if (condicionUsuaria == null)
                    {
                        return NotFound();
                    }

                    condicionUsuaria.Codigo = viewModel.Codigo;
                    condicionUsuaria.Nombre = viewModel.Nombre;
                    condicionUsuaria.Descripcion = viewModel.Descripcion;
                    condicionUsuaria.Estado = viewModel.Estado;

                    _context.Update(condicionUsuaria);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Condición usuaria actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CondicionUsuariaExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar la condición usuaria: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: CondicionUsuaria/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var condicionUsuaria = await _context.CondicionesUsuarias
                .FirstOrDefaultAsync(m => m.Id == id);

            if (condicionUsuaria == null)
            {
                return NotFound();
            }

            var viewModel = new CondicionUsuariaItemViewModel
            {
                Id = condicionUsuaria.Id,
                Codigo = condicionUsuaria.Codigo,
                Nombre = condicionUsuaria.Nombre,
                Descripcion = condicionUsuaria.Descripcion,
                Estado = condicionUsuaria.Estado,
                FechaCreacion = condicionUsuaria.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: CondicionUsuaria/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var condicionUsuaria = await _context.CondicionesUsuarias.FindAsync(id);
                if (condicionUsuaria != null)
                {
                    _context.CondicionesUsuarias.Remove(condicionUsuaria);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Condición usuaria eliminada exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la condición usuaria: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: CondicionUsuaria/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var condicionUsuaria = await _context.CondicionesUsuarias.FindAsync(id);
                if (condicionUsuaria != null)
                {
                    condicionUsuaria.Estado = !condicionUsuaria.Estado;
                    _context.Update(condicionUsuaria);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = condicionUsuaria.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Condición usuaria no encontrada" });
        }

        private bool CondicionUsuariaExists(int id)
        {
            return _context.CondicionesUsuarias.Any(e => e.Id == id);
        }
    }
}