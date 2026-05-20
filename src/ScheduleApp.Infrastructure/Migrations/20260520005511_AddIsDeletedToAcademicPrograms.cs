using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToAcademicPrograms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AcademicPrograms",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AcademicPrograms");
        }
    }
}
