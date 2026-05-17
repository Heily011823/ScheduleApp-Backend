using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTapsiRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTapsi",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "TapsiRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TapsiRules", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "TapsiRules",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "RuleType", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Las materias TAPSI no pueden superar 4 horas diarias.", true, "MAX_DAILY_HOURS", null, "4" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Las materias TAPSI solo pueden asignarse en días hábiles.", true, "ALLOWED_DAYS", null, "Lunes,Martes,Miércoles,Jueves,Viernes" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Las materias TAPSI deben dictarse entre 7:00 y 18:00.", true, "TIME_RANGE", null, "07:00-18:00" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TapsiRules");

            migrationBuilder.DropColumn(
                name: "IsTapsi",
                table: "Subjects");
        }
    }
}
