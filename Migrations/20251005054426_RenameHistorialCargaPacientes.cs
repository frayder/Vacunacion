using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Highdmin.Migrations
{
    /// <inheritdoc />
    public partial class RenameHistorialCargaPacientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistorialCargas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Usuario = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TotalCargados = table.Column<int>(type: "int", nullable: false),
                    TotalExistentes = table.Column<int>(type: "int", nullable: false),
                    ArchivoNombre = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialCargas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pacientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Eps = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoIdentificacion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PrimerNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegundoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrimerApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegundoApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pacientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegistrosVacunacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Consecutivo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NombresApellidos = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    NumeroDocumento = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Genero = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AseguradoraId = table.Column<int>(type: "int", nullable: true),
                    RegimenAfiliacionId = table.Column<int>(type: "int", nullable: true),
                    PertenenciaEtnicaId = table.Column<int>(type: "int", nullable: true),
                    CentroAtencionId = table.Column<int>(type: "int", nullable: true),
                    CondicionUsuariaId = table.Column<int>(type: "int", nullable: true),
                    TipoCarnetId = table.Column<int>(type: "int", nullable: true),
                    Vacuna = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroDosis = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaAplicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Lote = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Laboratorio = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ViaAdministracion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SitioAplicacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Vacunador = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    RegistroProfesional = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NotasFinales = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UsuarioCreadorId = table.Column<int>(type: "int", nullable: true),
                    UsuarioModificadorId = table.Column<int>(type: "int", nullable: true),
                    PacienteId = table.Column<int>(type: "int", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Email",
                table: "Pacientes",
                column: "Email",
                unique: true,
                filter: "[Email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pacientes_Identificacion",
                table: "Pacientes",
                column: "Identificacion",
                unique: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorialCargas");

            migrationBuilder.DropTable(
                name: "RegistrosVacunacion");

            migrationBuilder.DropTable(
                name: "Pacientes");
        }
    }
}
