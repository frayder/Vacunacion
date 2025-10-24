using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Highdmin.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_Postgres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empresas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    RazonSocial = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Nit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Direccion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empresas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Resource = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Icon = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ParentId = table.Column<int>(type: "integer", nullable: true),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    Controller = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Action = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuItems_MenuItems_ParentId",
                        column: x => x.ParentId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Aseguradoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aseguradoras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Aseguradoras_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CentrosAtencion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CentrosAtencion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CentrosAtencion_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CondicionesUsuarias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CondicionesUsuarias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CondicionesUsuarias_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HistorialCargaPacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaCarga = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Eps = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Usuario = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalCargados = table.Column<int>(type: "integer", nullable: false),
                    TotalExistentes = table.Column<int>(type: "integer", nullable: false),
                    ArchivoNombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCargaPacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistorialCargaPacientes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    RangoDosis = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Insumos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Eps = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Identificacion = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TipoIdentificacion = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    PrimerNombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SegundoNombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PrimerApellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SegundoApellido = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Genero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Sexo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pacientes_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    PermissionId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Resource = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.PermissionId);
                    table.ForeignKey(
                        name: "FK_Permissions_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PertenenciasEtnicas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PertenenciasEtnicas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PertenenciasEtnicas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegimenesAfiliacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegimenesAfiliacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegimenesAfiliacion_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nombre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TiposCarnet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposCarnet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TiposCarnet_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesRangoInsumo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InsumoId = table.Column<int>(type: "integer", nullable: false),
                    EdadMinima = table.Column<int>(type: "integer", nullable: false),
                    EdadMaxima = table.Column<int>(type: "integer", nullable: false),
                    UnidadMedidaEdadMinima = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    UnidadMedidaEdadMaxima = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dosis = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DescripcionRango = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Estado = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesRangoInsumo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfiguracionesRangoInsumo_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    MenuItemId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: true),
                    CanCreate = table.Column<bool>(type: "boolean", nullable: false),
                    CanRead = table.Column<bool>(type: "boolean", nullable: false),
                    CanUpdate = table.Column<bool>(type: "boolean", nullable: false),
                    CanDelete = table.Column<bool>(type: "boolean", nullable: false),
                    CanActivate = table.Column<bool>(type: "boolean", nullable: false),
                    CanResetPassword = table.Column<bool>(type: "boolean", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.MenuItemId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_MenuItems_MenuItemId",
                        column: x => x.MenuItemId,
                        principalTable: "MenuItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId");
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Entradas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FechaEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    InsumoId = table.Column<int>(type: "integer", nullable: false),
                    Cantidad = table.Column<int>(type: "integer", nullable: false),
                    UsuarioId = table.Column<int>(type: "integer", nullable: false),
                    Mes = table.Column<string>(type: "text", nullable: false),
                    Notas = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entradas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entradas_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entradas_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entradas_Users_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosVacunacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Consecutivo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    PrimerNombre = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SegundoNombre = table.Column<string>(type: "text", nullable: false),
                    PrimerApellido = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SegundoApellido = table.Column<string>(type: "text", nullable: false),
                    TipoDocumento = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Genero = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Telefono = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Direccion = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AseguradoraId = table.Column<int>(type: "integer", nullable: true),
                    RegimenAfiliacionId = table.Column<int>(type: "integer", nullable: true),
                    PertenenciaEtnicaId = table.Column<int>(type: "integer", nullable: true),
                    CentroAtencionId = table.Column<int>(type: "integer", nullable: true),
                    CondicionUsuariaId = table.Column<int>(type: "integer", nullable: true),
                    TipoCarnetId = table.Column<int>(type: "integer", nullable: true),
                    Vacuna = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    NotasFinales = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    FechaAtencion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    EsquemaCompleto = table.Column<bool>(type: "boolean", nullable: false),
                    Sexo = table.Column<string>(type: "text", nullable: true),
                    OrientacionSexual = table.Column<string>(type: "text", nullable: true),
                    EdadGestacional = table.Column<int>(type: "integer", nullable: true),
                    PesoInfante = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    PaisNacimiento = table.Column<string>(type: "text", nullable: true),
                    LugardeParto = table.Column<string>(type: "text", nullable: true),
                    EstatusMigratorio = table.Column<string>(type: "text", nullable: true),
                    Desplazado = table.Column<bool>(type: "boolean", nullable: false),
                    Discapacitado = table.Column<bool>(type: "boolean", nullable: false),
                    Fallecido = table.Column<bool>(type: "boolean", nullable: false),
                    VictimaConflictoArmado = table.Column<bool>(type: "boolean", nullable: false),
                    Estudia = table.Column<bool>(type: "boolean", nullable: false),
                    PaisResidencia = table.Column<string>(type: "text", nullable: true),
                    DepartamentoResidencia = table.Column<string>(type: "text", nullable: true),
                    MunicipioResidencia = table.Column<string>(type: "text", nullable: true),
                    ComunaLocalidad = table.Column<string>(type: "text", nullable: true),
                    Area = table.Column<string>(type: "text", nullable: true),
                    Celular = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    AutorizaLlamadas = table.Column<bool>(type: "boolean", nullable: false),
                    AutorizaEnvioCorreo = table.Column<bool>(type: "boolean", nullable: false),
                    Relacion = table.Column<string>(type: "text", nullable: true),
                    EnfermedadContraindicacionVacuna = table.Column<bool>(type: "boolean", nullable: true),
                    ReaccionBiologico = table.Column<bool>(type: "boolean", nullable: true),
                    MadreCuidador = table.Column<bool>(type: "boolean", nullable: true),
                    TipoIdentificacionCuidador = table.Column<string>(type: "text", nullable: true),
                    NumeroDocumentoCuidador = table.Column<string>(type: "text", nullable: true),
                    PrimerNombreCuidador = table.Column<string>(type: "text", nullable: true),
                    SegundoNombreCuidador = table.Column<string>(type: "text", nullable: true),
                    PrimerApellidoCuidador = table.Column<string>(type: "text", nullable: true),
                    SegundoApellidoCuidador = table.Column<string>(type: "text", nullable: true),
                    EmailCuidador = table.Column<string>(type: "text", nullable: true),
                    TelefonoCuidador = table.Column<string>(type: "text", nullable: true),
                    CelularCuidador = table.Column<string>(type: "text", nullable: true),
                    RegimenAfiliacionCuidador = table.Column<int>(type: "integer", nullable: true),
                    PertenenciaEtnicaIdCuidador = table.Column<int>(type: "integer", nullable: true),
                    EstadoDesplazadoCuidador = table.Column<bool>(type: "boolean", nullable: true),
                    ParentescoCuidador = table.Column<string>(type: "text", nullable: true),
                    Dosis = table.Column<string>(type: "text", nullable: true),
                    FechaAplicacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Responsable = table.Column<string>(type: "text", nullable: true),
                    IngresoPAIWEB = table.Column<bool>(type: "boolean", nullable: true),
                    CentroSaludResponsable = table.Column<string>(type: "text", nullable: true),
                    Estado = table.Column<bool>(type: "boolean", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UsuarioCreadorId = table.Column<int>(type: "integer", nullable: true),
                    UsuarioModificadorId = table.Column<int>(type: "integer", nullable: true),
                    NombreCompleto = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    MotivoNoIngresoPAIWEB = table.Column<string>(type: "text", nullable: true),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false),
                    MarcarComoPerdida = table.Column<bool>(type: "boolean", nullable: true),
                    MotivoPerdida = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    PacienteId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosVacunacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_Aseguradoras_AseguradoraId",
                        column: x => x.AseguradoraId,
                        principalTable: "Aseguradoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_CentrosAtencion_CentroAtencionId",
                        column: x => x.CentroAtencionId,
                        principalTable: "CentrosAtencion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_CondicionesUsuarias_CondicionUsuariaId",
                        column: x => x.CondicionUsuariaId,
                        principalTable: "CondicionesUsuarias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_Pacientes_PacienteId",
                        column: x => x.PacienteId,
                        principalTable: "Pacientes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_PertenenciasEtnicas_PertenenciaEtnicaId",
                        column: x => x.PertenenciaEtnicaId,
                        principalTable: "PertenenciasEtnicas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_RegimenesAfiliacion_RegimenAfiliacionId",
                        column: x => x.RegimenAfiliacionId,
                        principalTable: "RegimenesAfiliacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_TiposCarnet_TipoCarnetId",
                        column: x => x.TipoCarnetId,
                        principalTable: "TiposCarnet",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_Users_UsuarioCreadorId",
                        column: x => x.UsuarioCreadorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_RegistrosVacunacion_Users_UsuarioModificadorId",
                        column: x => x.UsuarioModificadorId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AntecedentesMedicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RegistroVacunacionId = table.Column<int>(type: "integer", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Tipo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Observaciones = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    FechaModificacion = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NumeroDocumentoPaciente = table.Column<string>(type: "text", nullable: false),
                    EmpresaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntecedentesMedicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AntecedentesMedicos_Empresas_EmpresaId",
                        column: x => x.EmpresaId,
                        principalTable: "Empresas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AntecedentesMedicos_RegistrosVacunacion_RegistroVacunacionId",
                        column: x => x.RegistroVacunacionId,
                        principalTable: "RegistrosVacunacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AntecedentesMedicos_EmpresaId",
                table: "AntecedentesMedicos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_AntecedentesMedicos_RegistroVacunacionId",
                table: "AntecedentesMedicos",
                column: "RegistroVacunacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Aseguradoras_Codigo",
                table: "Aseguradoras",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Aseguradoras_EmpresaId",
                table: "Aseguradoras",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_CentrosAtencion_Codigo",
                table: "CentrosAtencion",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CentrosAtencion_EmpresaId",
                table: "CentrosAtencion",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_CondicionesUsuarias_Codigo",
                table: "CondicionesUsuarias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CondicionesUsuarias_EmpresaId",
                table: "CondicionesUsuarias",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesRangoInsumo_InsumoId",
                table: "ConfiguracionesRangoInsumo",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Codigo",
                table: "Empresas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empresas_Nit",
                table: "Empresas",
                column: "Nit",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_EmpresaId",
                table: "Entradas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_InsumoId",
                table: "Entradas",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Entradas_UsuarioId",
                table: "Entradas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialCargaPacientes_EmpresaId",
                table: "HistorialCargaPacientes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_EmpresaId",
                table: "Insumos",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_ParentId",
                table: "MenuItems",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_Resource",
                table: "MenuItems",
                column: "Resource",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Email",
                table: "Pacientes",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_EmpresaId",
                table: "Pacientes",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Identificacion",
                table: "Pacientes",
                column: "Identificacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_EmpresaId",
                table: "Permissions",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Resource_Action",
                table: "Permissions",
                columns: new[] { "Resource", "Action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PertenenciasEtnicas_Codigo",
                table: "PertenenciasEtnicas",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PertenenciasEtnicas_EmpresaId",
                table: "PertenenciasEtnicas",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegimenesAfiliacion_Codigo",
                table: "RegimenesAfiliacion",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegimenesAfiliacion_EmpresaId",
                table: "RegimenesAfiliacion",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_AseguradoraId",
                table: "RegistrosVacunacion",
                column: "AseguradoraId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_CentroAtencionId",
                table: "RegistrosVacunacion",
                column: "CentroAtencionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_CondicionUsuariaId",
                table: "RegistrosVacunacion",
                column: "CondicionUsuariaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_Consecutivo",
                table: "RegistrosVacunacion",
                column: "Consecutivo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_EmpresaId",
                table: "RegistrosVacunacion",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_PacienteId",
                table: "RegistrosVacunacion",
                column: "PacienteId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_PertenenciaEtnicaId",
                table: "RegistrosVacunacion",
                column: "PertenenciaEtnicaId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_RegimenAfiliacionId",
                table: "RegistrosVacunacion",
                column: "RegimenAfiliacionId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_TipoCarnetId",
                table: "RegistrosVacunacion",
                column: "TipoCarnetId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_TipoDocumento_NumeroDocumento",
                table: "RegistrosVacunacion",
                columns: new[] { "TipoDocumento", "NumeroDocumento" });

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_UsuarioCreadorId",
                table: "RegistrosVacunacion",
                column: "UsuarioCreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RegistrosVacunacion_UsuarioModificadorId",
                table: "RegistrosVacunacion",
                column: "UsuarioModificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_EmpresaId",
                table: "RolePermissions",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_MenuItemId",
                table: "RolePermissions",
                column: "MenuItemId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_EmpresaId",
                table: "Roles",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposCarnet_Codigo",
                table: "TiposCarnet",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TiposCarnet_EmpresaId",
                table: "TiposCarnet",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_EmpresaId",
                table: "UserRoles",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_EmpresaId",
                table: "Users",
                column: "EmpresaId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AntecedentesMedicos");

            migrationBuilder.DropTable(
                name: "ConfiguracionesRangoInsumo");

            migrationBuilder.DropTable(
                name: "Entradas");

            migrationBuilder.DropTable(
                name: "HistorialCargaPacientes");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "RegistrosVacunacion");

            migrationBuilder.DropTable(
                name: "Insumos");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Aseguradoras");

            migrationBuilder.DropTable(
                name: "CentrosAtencion");

            migrationBuilder.DropTable(
                name: "CondicionesUsuarias");

            migrationBuilder.DropTable(
                name: "Pacientes");

            migrationBuilder.DropTable(
                name: "PertenenciasEtnicas");

            migrationBuilder.DropTable(
                name: "RegimenesAfiliacion");

            migrationBuilder.DropTable(
                name: "TiposCarnet");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Empresas");
        }
    }
}
