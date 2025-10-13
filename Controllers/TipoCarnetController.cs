using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class TipoCarnetController : BaseEmpresaController
    {
        private readonly ApplicationDbContext _context;

        public TipoCarnetController(ApplicationDbContext context, IEmpresaService empresaService ) : base(empresaService)
        {
            _context = context;
        }

        // GET: TipoCarnet
        public async Task<IActionResult> Index()
        {
            try
            {
                var tiposCarnet = await _context.TiposCarnet
                    .Where(t => t.EmpresaId == CurrentEmpresaId)
                    .OrderBy(t => t.Codigo)
                    .Select(t => new TipoCarnetItemViewModel
                    {
                        Id = t.Id,
                        Codigo = t.Codigo,
                        Nombre = t.Nombre,
                        Descripcion = t.Descripcion,
                        Estado = t.Estado,
                        FechaCreacion = t.FechaCreacion
                    })
                    .ToListAsync();

                var viewModel = new TipoCarnetViewModel
                {
                    TotalTipos = tiposCarnet.Count,
                    TiposActivos = tiposCarnet.Count(t => t.Estado),
                    TiposInactivos = tiposCarnet.Count(t => !t.Estado),
                    TiposCarnet = tiposCarnet
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los tipos de carnet: " + ex.Message;
                return View(new TipoCarnetViewModel());
            }
        }

        // GET: TipoCarnet/Crear
        public IActionResult Crear()
        {
            return View(new TipoCarnetCreateViewModel());
        }

        // POST: TipoCarnet/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoCarnetCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe
                    var existeCodigo = await _context.TiposCarnet
                        .AnyAsync(t => t.Codigo.ToUpper() == model.Codigo.ToUpper());

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un tipo de carnet con este código");
                        return View(model);
                    }

                    var tipoCarnet = new TipoCarnet
                    {
                        Codigo = model.Codigo.ToUpper(),
                        Nombre = model.Nombre,
                        Descripcion = model.Descripcion,
                        Estado = model.Estado,
                        FechaCreacion = DateTime.Now,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.TiposCarnet.Add(tipoCarnet);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Tipo de carnet creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el tipo de carnet: " + ex.Message);
                }
            }

            return View(model);
        }

        // GET: TipoCarnet/Editar/5
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var tipoCarnet = await _context.TiposCarnet.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (tipoCarnet == null)
                {
                    return NotFound();
                }

                var model = new TipoCarnetEditViewModel
                {
                    Id = tipoCarnet.Id,
                    Codigo = tipoCarnet.Codigo,
                    Nombre = tipoCarnet.Nombre,
                    Descripcion = tipoCarnet.Descripcion,
                    Estado = tipoCarnet.Estado,
                    FechaCreacion = tipoCarnet.FechaCreacion
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el tipo de carnet: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TipoCarnet/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, TipoCarnetEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si el código ya existe en otro registro
                    var existeCodigo = await _context.TiposCarnet
                        .AnyAsync(t => t.Codigo.ToUpper() == model.Codigo.ToUpper() && t.Id != id);

                    if (existeCodigo)
                    {
                        ModelState.AddModelError("Codigo", "Ya existe un tipo de carnet con este código");
                        return View(model);
                    }

                    var tipoCarnet = await _context.TiposCarnet.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (tipoCarnet == null)
                    {
                        return NotFound();
                    }

                    tipoCarnet.Codigo = model.Codigo.ToUpper();
                    tipoCarnet.Nombre = model.Nombre;
                    tipoCarnet.Descripcion = model.Descripcion;
                    tipoCarnet.Estado = model.Estado;

                    _context.Update(tipoCarnet);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Tipo de carnet actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoCarnetExists(model.Id))
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
                    ModelState.AddModelError("", "Error al actualizar el tipo de carnet: " + ex.Message);
                }
            }

            return View(model);
        }

        // POST: TipoCarnet/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var tipoCarnet = await _context.TiposCarnet.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (tipoCarnet != null)
                {
                    _context.TiposCarnet.Remove(tipoCarnet);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Tipo de carnet eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se encontró el tipo de carnet";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el tipo de carnet: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TipoCarnetExists(int id)
        {
            return _context.TiposCarnet.Any(e => e.Id == id);
        }
    }
}