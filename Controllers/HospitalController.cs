using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;

namespace Highdmin.Controllers
{
    public class HospitalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HospitalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Hospital
        public async Task<IActionResult> Index()
        {
            try
            {
                var hospitales = await _context.Hospitales
                    .OrderBy(h => h.Codigo)
                    .Select(h => new HospitalItemViewModel
                    {
                        Id = h.Id,
                        Codigo = h.Codigo,
                        Nombre = h.Nombre,
                        Descripcion = h.Descripcion,
                        Estado = h.Estado,
                        FechaCreacion = h.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new HospitalViewModel
                {
                    TotalHospitales = hospitales.Count,
                    HospitalesActivos = hospitales.Count(h => h.Estado),
                    HospitalesInactivos = hospitales.Count(h => !h.Estado),
                    Hospitales = hospitales
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los hospitales: " + ex.Message;
                return View(new HospitalViewModel());
            }
        }

        // GET: Hospital/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitales
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hospital == null)
            {
                return NotFound();
            }

            var viewModel = new HospitalItemViewModel
            {
                Id = hospital.Id,
                Codigo = hospital.Codigo,
                Nombre = hospital.Nombre,
                Descripcion = hospital.Descripcion,
                Estado = hospital.Estado,
                FechaCreacion = hospital.FechaCreacion
            };

            return View(viewModel);
        }

        // GET: Hospital/Create
        public IActionResult Create()
        {
            return View(new HospitalCreateViewModel());
        }

        // POST: Hospital/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HospitalCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.Hospitales
                        .AnyAsync(h => h.Codigo == viewModel.Codigo);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un hospital con este código.");
                        return View(viewModel);
                    }

                    var hospital = new Hospital
                    {
                        Codigo = viewModel.Codigo,
                        Nombre = viewModel.Nombre,
                        Descripcion = viewModel.Descripcion,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Add(hospital);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Hospital creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear el hospital: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: Hospital/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitales.FindAsync(id);
            if (hospital == null)
            {
                return NotFound();
            }

            var viewModel = new HospitalEditViewModel
            {
                Id = hospital.Id,
                Codigo = hospital.Codigo,
                Nombre = hospital.Nombre,
                Descripcion = hospital.Descripcion,
                Estado = hospital.Estado,
                FechaCreacion = hospital.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: Hospital/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, HospitalEditViewModel viewModel)
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
                    var existeCodigo = await _context.Hospitales
                        .AnyAsync(h => h.Codigo == viewModel.Codigo && h.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un hospital con este código.");
                        return View(viewModel);
                    }

                    var hospital = await _context.Hospitales.FindAsync(id);
                    if (hospital == null)
                    {
                        return NotFound();
                    }

                    hospital.Codigo = viewModel.Codigo;
                    hospital.Nombre = viewModel.Nombre;
                    hospital.Descripcion = viewModel.Descripcion;
                    hospital.Estado = viewModel.Estado;

                    _context.Update(hospital);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Hospital actualizado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HospitalExists(viewModel.Id))
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
                    TempData["Error"] = "Error al actualizar el hospital: " + ex.Message;
                }
            }

            return View(viewModel);
        }

        // GET: Hospital/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hospital = await _context.Hospitales
                .FirstOrDefaultAsync(m => m.Id == id);

            if (hospital == null)
            {
                return NotFound();
            }

            var viewModel = new HospitalItemViewModel
            {
                Id = hospital.Id,
                Codigo = hospital.Codigo,
                Nombre = hospital.Nombre,
                Descripcion = hospital.Descripcion,
                Estado = hospital.Estado,
                FechaCreacion = hospital.FechaCreacion
            };

            return View(viewModel);
        }

        // POST: Hospital/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var hospital = await _context.Hospitales.FindAsync(id);
                if (hospital != null)
                {
                    _context.Hospitales.Remove(hospital);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Hospital eliminado exitosamente.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el hospital: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Hospital/ToggleEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            try
            {
                var hospital = await _context.Hospitales.FindAsync(id);
                if (hospital != null)
                {
                    hospital.Estado = !hospital.Estado;
                    _context.Update(hospital);
                    await _context.SaveChangesAsync();

                    return Json(new { success = true, estado = hospital.Estado });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false, message = "Hospital no encontrado" });
        }

        private bool HospitalExists(int id)
        {
            return _context.Hospitales.Any(e => e.Id == id);
        }
    }
}