using Microsoft.EntityFrameworkCore;
using Highdmin.Models;

namespace Highdmin.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para las entidades RBAC
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<TipoCarnet> TiposCarnet { get; set; }
        public DbSet<CondicionUsuaria> CondicionesUsuarias { get; set; }
        public DbSet<PertenenciaEtnica> PertenenciasEtnicas { get; set; }
        public DbSet<Aseguradora> Aseguradoras { get; set; }
        public DbSet<RegimenAfiliacion> RegimenesAfiliacion { get; set; }
        public DbSet<Hospital> Hospitales { get; set; }
        public DbSet<CentroAtencion> CentrosAtencion { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<Entrada> Entradas { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Configuración de la entidad Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(255);
            });

            // Configuración de UserRole
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(ur => new { ur.UserId, ur.RoleId });
                entity.HasOne(ur => ur.User)
                      .WithMany(u => u.UserRoles)
                      .HasForeignKey(ur => ur.UserId);
                entity.HasOne(ur => ur.Role)
                      .WithMany(r => r.UserRoles)
                      .HasForeignKey(ur => ur.RoleId);
            });

            // Configuración de RolePermission
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasKey(rp => new { rp.RoleId, rp.MenuItemId });
                entity.HasOne(rp => rp.Role)
                      .WithMany(r => r.RolePermissions)
                      .HasForeignKey(rp => rp.RoleId);
                entity.HasOne(rp => rp.MenuItem)
                      .WithMany()
                      .HasForeignKey(rp => rp.MenuItemId);
                entity.HasOne(rp => rp.Permission)
                      .WithMany()
                      .HasForeignKey(rp => rp.PermissionId)
                      .IsRequired(false);
            });

            // Configuración de Permission
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(e => e.PermissionId);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Action).IsRequired().HasMaxLength(50);
                entity.HasIndex(p => new { p.Resource, p.Action }).IsUnique();
            });

            // Configuración de MenuItem
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Resource).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Icon).HasMaxLength(50);
                entity.Property(e => e.Url).HasMaxLength(255);
                entity.Property(e => e.Controller).HasMaxLength(100);
                entity.Property(e => e.Action).HasMaxLength(100);
                entity.HasIndex(m => m.Resource).IsUnique();

                // Configuración de la relación jerárquica
                entity.HasOne(m => m.Parent)
                      .WithMany(m => m.Children)
                      .HasForeignKey(m => m.ParentId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TipoCarnet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });

            modelBuilder.Entity<CondicionUsuaria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });

             modelBuilder.Entity<PertenenciaEtnica>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });

            modelBuilder.Entity<Aseguradora>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });

            modelBuilder.Entity<RegimenAfiliacion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });

            modelBuilder.Entity<Hospital>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });

            modelBuilder.Entity<CentroAtencion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Tipo).HasMaxLength(500);
                entity.HasIndex(t => t.Codigo).IsUnique();
            });
        }
    }
}