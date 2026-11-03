using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaEventos.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEventosAsientosComprasBoletos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Lugar = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    PrecioEntrada = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Asientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    Fila = table.Column<int>(type: "int", nullable: false),
                    Columna = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asientos", x => x.Id);
                    table.UniqueConstraint("AK_Asientos_Id_EventoId", x => new { x.Id, x.EventoId });
                    table.ForeignKey(
                        name: "FK_Asientos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Total = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compras", x => x.Id);
                    table.UniqueConstraint("AK_Compras_Id_EventoId", x => new { x.Id, x.EventoId });
                    table.ForeignKey(
                        name: "FK_Compras_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Compras_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Boletos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompraId = table.Column<int>(type: "int", nullable: false),
                    AsientoId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boletos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boletos_Asientos_AsientoId_EventoId",
                        columns: x => new { x.AsientoId, x.EventoId },
                        principalTable: "Asientos",
                        principalColumns: new[] { "Id", "EventoId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Boletos_Compras_CompraId_EventoId",
                        columns: x => new { x.CompraId, x.EventoId },
                        principalTable: "Compras",
                        principalColumns: new[] { "Id", "EventoId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Asientos_EventoId_Fila_Columna",
                table: "Asientos",
                columns: new[] { "EventoId", "Fila", "Columna" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boletos_AsientoId",
                table: "Boletos",
                column: "AsientoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Boletos_AsientoId_EventoId",
                table: "Boletos",
                columns: new[] { "AsientoId", "EventoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Boletos_CompraId_EventoId",
                table: "Boletos",
                columns: new[] { "CompraId", "EventoId" });

            migrationBuilder.CreateIndex(
                name: "IX_Compras_EventoId",
                table: "Compras",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_UsuarioId",
                table: "Compras",
                column: "UsuarioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Boletos");

            migrationBuilder.DropTable(
                name: "Asientos");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
