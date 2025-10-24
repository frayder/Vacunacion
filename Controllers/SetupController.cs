using Microsoft.AspNetCore.Mvc;
using Highdmin.Data;
using Highdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace Highdmin.Controllers
{
    public class SetupController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SetupController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> InitializeRBAC()
        {
            try
            {
                // Crear las tablas RBAC si no existen usando SQL directo
                await CreateRBACTablesIfNotExist();

                // Verificar si ya existen MenuItems
                var existingMenuItems = await _context.MenuItems.AnyAsync();
                if (!existingMenuItems)
                {
                    // Insertar MenuItems de ejemplo
                    var menuItems = new List<MenuItem>
                    {
                        new MenuItem { Name = "Dashboard", Resource = "Dashboard" },
                        new MenuItem { Name = "Peticiones", Resource = "Peticiones" },
                        new MenuItem { Name = "Facturas", Resource = "Facturas" },
                        new MenuItem { Name = "Usuarios", Resource = "Usuarios" },
                        new MenuItem { Name = "Roles", Resource = "Roles" },
                        new MenuItem { Name = "Reportes", Resource = "Reportes" },
                        new MenuItem { Name = "Configuración", Resource = "Configuracion" },
                        new MenuItem { Name = "Órdenes", Resource = "Ordenes" },
                        new MenuItem { Name = "Pacientes", Resource = "Pacientes" },
                        new MenuItem { Name = "Comentarios", Resource = "Comentarios" }
                    };

                    _context.MenuItems.AddRange(menuItems);
                    await _context.SaveChangesAsync();
                }

                // Verificar si ya existen roles
                var existingRoles = await _context.Roles.AnyAsync();
                if (!existingRoles)
                {
                    // Insertar roles de ejemplo
                    var roles = new List<Role>
                    {
                        new Role { Nombre = "Administrador", Descripcion = "Rol con acceso completo al sistema" },
                        new Role { Nombre = "Supervisor", Descripcion = "Rol con permisos de supervisión y aprobación" },
                        new Role { Nombre = "Operador", Descripcion = "Rol básico para operaciones diarias" },
                        new Role { Nombre = "Consulta", Descripcion = "Rol de solo lectura para consultas" }
                    };

                    _context.Roles.AddRange(roles);
                    await _context.SaveChangesAsync();

                    // Asignar permisos al rol Administrador
                    var adminRole = await _context.Roles.FirstAsync(r => r.Nombre == "Administrador");
                    var menuItems = await _context.MenuItems.ToListAsync();

                    var adminPermissions = menuItems.Select(mi => new RolePermission
                    {
                        RoleId = adminRole.Id,
                        MenuItemId = mi.Id,
                        CanCreate = true,
                        CanRead = true,
                        CanUpdate = true,
                        CanDelete = true
                    }).ToList();

                    _context.RolePermissions.AddRange(adminPermissions);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Sistema RBAC inicializado correctamente con datos de ejemplo.";
                return RedirectToAction("Index", "Roles");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error inicializando RBAC: {ex.Message}";
                return RedirectToAction("Index", "Roles");
            }
        }

        private async Task CreateRBACTablesIfNotExist()
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var commands = new[]
            {
                // Crear esquema si no existe
                "IF SCHEMA_ID(N'AnulacionFacturas') IS NULL EXEC(N'CREATE SCHEMA [AnulacionFacturas];')",
                
                // Crear tabla MenuItems
                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AnulacionFacturas' AND TABLE_NAME = 'MenuItems')
                BEGIN
                    CREATE TABLE [AnulacionFacturas].[MenuItems] (
                        [Id] int SERIAL(1,1) NOT NULL,
                        [Name] nvarchar(100) NOT NULL,
                        [Resource] nvarchar(255) NOT NULL,
                        CONSTRAINT [PK_MenuItems] PRIMARY KEY ([Id])
                    );
                    CREATE UNIQUE INDEX [IX_MenuItems_Resource] ON [AnulacionFacturas].[MenuItems] ([Resource]);
                END",
                
                // Crear tabla Roles
                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AnulacionFacturas' AND TABLE_NAME = 'Roles')
                BEGIN
                    CREATE TABLE [AnulacionFacturas].[Roles] (
                        [Id] int SERIAL(1,1) NOT NULL,
                        [Nombre] nvarchar(100) NOT NULL,
                        [Descripcion] nvarchar(255) NULL,
                        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
                    );
                END",
                
                // Crear tabla Permissions
                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AnulacionFacturas' AND TABLE_NAME = 'Permissions')
                BEGIN
                    CREATE TABLE [AnulacionFacturas].[Permissions] (
                        [PermissionId] int SERIAL(1,1) NOT NULL,
                        [Resource] nvarchar(255) NOT NULL,
                        [Action] nvarchar(50) NOT NULL,
                        CONSTRAINT [PK_Permissions] PRIMARY KEY ([PermissionId])
                    );
                    CREATE UNIQUE INDEX [IX_Permissions_Resource_Action] ON [AnulacionFacturas].[Permissions] ([Resource], [Action]);
                END",
                
                // Crear tabla RolePermissions
                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AnulacionFacturas' AND TABLE_NAME = 'RolePermissions')
                BEGIN
                    CREATE TABLE [AnulacionFacturas].[RolePermissions] (
                        [RoleId] int NOT NULL,
                        [MenuItemId] int NOT NULL,
                        [PermissionId] int NULL,
                        [CanCreate] boolean NOT NULL DEFAULT 0,
                        [CanRead] boolean NOT NULL DEFAULT 0,
                        [CanUpdate] boolean NOT NULL DEFAULT 0,
                        [CanDelete] boolean NOT NULL DEFAULT 0,
                        [CanApprove] boolean NOT NULL DEFAULT 0,
                        [CanReject] boolean NOT NULL DEFAULT 0,
                        [CanAssign] boolean NOT NULL DEFAULT 0,
                        [CanComment] boolean NOT NULL DEFAULT 0,
                        [CanAnnull] boolean NOT NULL DEFAULT 0,
                        [CanProcess] boolean NOT NULL DEFAULT 0,
                        [CanActivate] boolean NOT NULL DEFAULT 0,
                        [CanResetPassword] boolean NOT NULL DEFAULT 0,
                        CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [MenuItemId]),
                        CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AnulacionFacturas].[Roles] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_RolePermissions_MenuItems_MenuItemId] FOREIGN KEY ([MenuItemId]) REFERENCES [AnulacionFacturas].[MenuItems] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [AnulacionFacturas].[Permissions] ([PermissionId])
                    );
                    CREATE INDEX [IX_RolePermissions_MenuItemId] ON [AnulacionFacturas].[RolePermissions] ([MenuItemId]);
                    CREATE INDEX [IX_RolePermissions_PermissionId] ON [AnulacionFacturas].[RolePermissions] ([PermissionId]);
                END",
                
                // Crear tabla UserRoles
                @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'AnulacionFacturas' AND TABLE_NAME = 'UserRoles')
                BEGIN
                    CREATE TABLE [AnulacionFacturas].[UserRoles] (
                        [UserId] int NOT NULL,
                        [RoleId] int NOT NULL,
                        CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
                        CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AnulacionFacturas].[Roles] ([Id]) ON DELETE CASCADE
                    );
                    CREATE INDEX [IX_UserRoles_RoleId] ON [AnulacionFacturas].[UserRoles] ([RoleId]);
                END"
            };

            foreach (var command in commands)
            {
                using var cmd = connection.CreateCommand();
                cmd.CommandText = command;
                await cmd.ExecuteNonQueryAsync();
            }

            await connection.CloseAsync();
        }
    }
}
