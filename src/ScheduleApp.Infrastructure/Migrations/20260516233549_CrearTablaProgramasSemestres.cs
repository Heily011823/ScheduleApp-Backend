using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaProgramasSemestres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgramSemesters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicProgramId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SemesterNumber = table.Column<int>(type: "int", nullable: false),
                    MaxCredits = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramSemesters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramSemesters_AcademicPrograms_AcademicProgramId",
                        column: x => x.AcademicProgramId,
                        principalTable: "AcademicPrograms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramSemesters_AcademicProgramId",
                table: "ProgramSemesters",
                column: "AcademicProgramId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramSemesters");
        }
    }
}
