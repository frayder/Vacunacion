using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Highdmin.Data;
using Highdmin.Models;
using Highdmin.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Highdmin.Services;

namespace Highdmin.Controllers
{
    public class EntradaController : BaseAuthorizationController
    {
        private readonly ApplicationDbContext _context;

        public EntradaController(ApplicationDbContext context, IEmpresaService empresaService, AuthorizationService authorizationService) : base(empresaService, authorizationService)
        {
            _context = context;
        } 

        // GET: Entrada
        public async Task<IActionResult> Index()
        {
            try
            {
                // Validar permisos y obtener todos los permisos del módulo
                var (redirect, permissions) = await ValidateAndGetPermissionsAsync("Entradas", "Read");
                if (redirect != null) return redirect;

                var entradas = await _context.Entradas
                    .Where(e => e.EmpresaId == CurrentEmpresaId)
                    .Include(e => e.Insumo)
                    .Include(e => e.Usuario)
                    .OrderByDescending(e => e.FechaEntrada)
                    .Select(e => new EntradaItemViewModel
                    {
                        Id = e.Id,
                        FechaEntrada = e.FechaEntrada,
                        InsumoNombre = e.Insumo != null ? e.Insumo.Nombre : "N/A",
                        InsumoCodigo = e.Insumo != null ? e.Insumo.Codigo : "N/A",
                        InsumoTipo = e.Insumo != null ? e.Insumo.Tipo : "N/A",
                        Cantidad = e.Cantidad,
                        UsuarioNombre = e.Usuario != null ? e.Usuario.UserName : "N/A",
                        Mes = e.Mes,
                        Notas = e.Notas,
                        Estado = e.Estado,
                        FechaCreacion = e.FechaCreacion
                    })
                    .ToListAsync();

                var mesActual = DateTime.Now.ToString("yyyy-MM");
                var entradasDelMes = entradas.Count(e => e.FechaEntrada.ToString("yyyy-MM") == mesActual);
                var insumosDiferentes = entradas.Select(e => e.InsumoCodigo).Distinct().Count();
                var usuariosActivos = await _context.Users.CountAsync(u => u.IsActive);

                var viewModel = new EntradaViewModel
                {
                    TotalEntradas = entradas.Count,
                    EntradasDelMes = entradasDelMes,
                    InsumosDiferentes = insumosDiferentes,
                    UsuariosActivos = usuariosActivos,
                    Entradas = entradas,
                    // Agregar permisos al ViewModel
                    CanCreate = permissions["Create"],
                    CanUpdate = permissions["Update"],
                    CanDelete = permissions["Delete"]
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar las entradas: " + ex.Message;
                return View(new EntradaViewModel());
            }
        }

        // GET: Entrada/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new EntradaCreateViewModel
            {
                FechaEntrada = DateTime.UtcNow,
                Mes = DateTime.Now.ToString("yyyy-MM")
            };

            await CargarListasSeleccion(viewModel);
            return View(viewModel);
        }

        // POST: Entrada/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EntradaCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var entrada = new Entrada
                    {
                        FechaEntrada = viewModel.FechaEntrada,
                        InsumoId = viewModel.InsumoId,
                        Cantidad = viewModel.Cantidad,
                        UsuarioId = viewModel.UsuarioId,
                        Mes = viewModel.Mes,
                        Notas = viewModel.Notas,
                        Estado = viewModel.Estado,
                        FechaCreacion = DateTime.UtcNow,
                        EmpresaId = CurrentEmpresaId
                    };

                    _context.Add(entrada);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Entrada registrada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear la entrada: " + ex.Message);
                }
            }

            await CargarListasSeleccion(viewModel);
            return View(viewModel);
        }

        // GET: Entrada/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrada = await _context.Entradas
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
            if (entrada == null)
            {
                return NotFound();
            }

            var viewModel = new EntradaEditViewModel
            {
                Id = entrada.Id,
                FechaEntrada = entrada.FechaEntrada,
                InsumoId = entrada.InsumoId,
                Cantidad = entrada.Cantidad,
                UsuarioId = entrada.UsuarioId,
                Mes = entrada.Mes,
                Notas = entrada.Notas,
                Estado = entrada.Estado,
                FechaCreacion = entrada.FechaCreacion
            };

            await CargarListasSeleccion(viewModel);
            return View(viewModel);
        }

        // POST: Entrada/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EntradaEditViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var entrada = await _context.Entradas
                        .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                    if (entrada == null)
                    {
                        return NotFound();
                    }

                    entrada.FechaEntrada = viewModel.FechaEntrada;
                    entrada.InsumoId = viewModel.InsumoId;
                    entrada.Cantidad = viewModel.Cantidad;
                    entrada.UsuarioId = viewModel.UsuarioId;
                    entrada.Mes = viewModel.Mes;
                    entrada.Notas = viewModel.Notas;
                    entrada.Estado = viewModel.Estado;

                    _context.Update(entrada);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Entrada actualizada exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntradaExists(viewModel.Id))
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
                    ModelState.AddModelError("", "Error al actualizar la entrada: " + ex.Message);
                }
            }

            await CargarListasSeleccion(viewModel);
            return View(viewModel);
        }

        // GET: Entrada/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entrada = await _context.Entradas
                .Include(e => e.Insumo)
                .Include(e => e.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);

            if (entrada == null)
            {
                return NotFound();
            }

            var viewModel = new EntradaItemViewModel
            {
                Id = entrada.Id,
                FechaEntrada = entrada.FechaEntrada,
                InsumoNombre = entrada.Insumo?.Nombre ?? "N/A",
                InsumoCodigo = entrada.Insumo?.Codigo ?? "N/A",
                InsumoTipo = entrada.Insumo?.Tipo ?? "N/A",
                Cantidad = entrada.Cantidad,
                UsuarioNombre = entrada.Usuario?.UserName ?? "N/A",
                Mes = entrada.Mes,
                Notas = entrada.Notas,
                Estado = entrada.Estado,
                FechaCreacion = entrada.FechaCreacion
            };

            return View(viewModel);
        }

        

        // POST: Entrada/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                var entrada = await _context.Entradas.FirstOrDefaultAsync(m => m.Id == id && m.EmpresaId == CurrentEmpresaId);
                if (entrada != null)
                {
                    _context.Entradas.Remove(entrada);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Entrada eliminada exitosamente.";
                }
                else
                {
                    TempData["Error"] = "No se encontró la entrada a eliminar.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la entrada: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        private bool EntradaExists(int id)
        {
            return _context.Entradas.Any(e => e.Id == id);
        }

        private async Task CargarListasSeleccion(EntradaCreateViewModel viewModel)
        {
            // Cargar insumos activos
            var insumos = await _context.Insumos
                .Where(i => i.Estado)
                .OrderBy(i => i.Nombre)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Codigo} - {i.Nombre}"
                })
                .ToListAsync();

            viewModel.Insumos = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione un insumo" }
            };
            viewModel.Insumos.AddRange(insumos);

            // Cargar usuarios activos
            var usuarios = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.UserName
                })
                .ToListAsync();

            viewModel.Usuarios = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione un usuario" }
            };
            viewModel.Usuarios.AddRange(usuarios);

            // Cargar meses
            viewModel.Meses = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos los insumos" },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-01"), Text = "Enero " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-02"), Text = "Febrero " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-03"), Text = "Marzo " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-04"), Text = "Abril " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-05"), Text = "Mayo " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-06"), Text = "Junio " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-07"), Text = "Julio " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-08"), Text = "Agosto " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-09"), Text = "Septiembre " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-10"), Text = "Octubre " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-11"), Text = "Noviembre " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-12"), Text = "Diciembre " + DateTime.Now.Year }
            };
        }

        private async Task CargarListasSeleccion(EntradaEditViewModel viewModel)
        {
            // Cargar insumos activos
            var insumos = await _context.Insumos
                .Where(i => i.Estado)
                .OrderBy(i => i.Nombre)
                .Select(i => new SelectListItem
                {
                    Value = i.Id.ToString(),
                    Text = $"{i.Codigo} - {i.Nombre}"
                })
                .ToListAsync();

            viewModel.Insumos = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione un insumo" }
            };
            viewModel.Insumos.AddRange(insumos);

            // Cargar usuarios activos
            var usuarios = await _context.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.UserName)
                .Select(u => new SelectListItem
                {
                    Value = u.UserId.ToString(),
                    Text = u.UserName
                })
                .ToListAsync();

            viewModel.Usuarios = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione un usuario" }
            };
            viewModel.Usuarios.AddRange(usuarios);

            // Cargar meses
            viewModel.Meses = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Todos los insumos" },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-01"), Text = "Enero " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-02"), Text = "Febrero " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-03"), Text = "Marzo " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-04"), Text = "Abril " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-05"), Text = "Mayo " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-06"), Text = "Junio " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-07"), Text = "Julio " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-08"), Text = "Agosto " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-09"), Text = "Septiembre " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-10"), Text = "Octubre " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-11"), Text = "Noviembre " + DateTime.Now.Year },
                new SelectListItem { Value = DateTime.Now.ToString("yyyy-12"), Text = "Diciembre " + DateTime.Now.Year }
            };
        }
    }
}