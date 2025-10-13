using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Highdmin.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Highdmin.Controllers
{
    [Authorize]
    public class TipoCarnetController : BaseEmpresaController
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthorizationService _authorizationService;

        public TipoCarnetController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService)
        {
            _context = context;
            _authorizationService = authorizationService;
        }

        // GET: TipoCarnet
        public async Task<IActionResult> Index()
        {
            // Verificar permiso de lectura
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasReadPermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Read");
            
            if (!hasReadPermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para ver los tipos de carnet.";
                return RedirectToAction("Index", "Dashboard");
            }

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

                // Obtener permisos del usuario para la vista
                var permissions = await _authorizationService.GetUserPermissionsAsync(userId, "TipoCarnet");

                var viewModel = new TipoCarnetViewModel
                {
                    TotalTipos = tiposCarnet.Count,
                    TiposActivos = tiposCarnet.Count(t => t.Estado),
                    TiposInactivos = tiposCarnet.Count(t => !t.Estado),
                    TiposCarnet = tiposCarnet,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
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
        public async Task<IActionResult> Crear()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasCreatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Create");
            
            if (!hasCreatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para crear tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

            return View(new TipoCarnetCreateViewModel());
        }

        // POST: TipoCarnet/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoCarnetCreateViewModel model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasCreatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Create");
            
            if (!hasCreatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para crear tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasUpdatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Update");
            
            if (!hasUpdatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para editar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasUpdatePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Update");
            
            if (!hasUpdatePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para editar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

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
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var hasDeletePermission = await _authorizationService.HasPermissionAsync(userId, "TipoCarnet", "Delete");
            
            if (!hasDeletePermission)
            {
                TempData["ErrorMessage"] = "No tiene permisos para eliminar tipos de carnet.";
                return RedirectToAction(nameof(Index));
            }

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