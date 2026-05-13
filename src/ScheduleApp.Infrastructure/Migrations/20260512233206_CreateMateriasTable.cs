using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateMateriasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Semestre = table.Column<int>(type: "int", nullable: false),
                    Creditos = table.Column<int>(type: "int", nullable: false),
                    HorasSemanales = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Materias",
                columns: new[] { "Id", "Codigo", "CreatedAt", "Creditos", "HorasSemanales", "IsActive", "Nombre", "Semestre" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-1111-1111-1111-111111111111"), "MAT101", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 4, true, "Matematicas I", 1 },
                    { new Guid("aaaaaaaa-2222-2222-2222-222222222222"), "ING201", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 3, 4, true, "Ingenieria de Software", 3 },
                    { new Guid("aaaaaaaa-3333-3333-3333-333333333333"), "BD401", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 4, 6, true, "Bases de Datos", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materias_Codigo",
                table: "Materias",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Materias");
        }
    }
}
