using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpecialtyIdToTeacher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Specialties");

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "Teachers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_SpecialtyId",
                table: "Teachers",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teachers_Specialties_SpecialtyId",
                table: "Teachers",
                column: "SpecialtyId",
                principalTable: "Specialties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teachers_Specialties_SpecialtyId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_SpecialtyId",
                table: "Teachers");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Teachers");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Specialties",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }
    }
}
