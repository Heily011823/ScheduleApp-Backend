using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsDeletedToSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Subjects",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Subjects");
        }
    }
}
