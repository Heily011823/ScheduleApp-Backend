using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherSpecialtyManyToMany : Migration
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

            migrationBuilder.AddColumn<Guid>(
                name: "SubjectId1",
                table: "Schedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SubjectRestrictions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RestrictionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectRestrictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectRestrictions_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubjectSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartHour = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndHour = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubjectSchedules_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherSpecialties",
                columns: table => new
                {
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecialtyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSpecialties", x => new { x.TeacherId, x.SpecialtyId });
                    table.ForeignKey(
                        name: "FK_TeacherSpecialties_Specialties_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSpecialties_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_SubjectId1",
                table: "Schedules",
                column: "SubjectId1");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectRestrictions_SubjectId",
                table: "SubjectRestrictions",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectSchedules_SubjectId",
                table: "SubjectSchedules",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSpecialties_SpecialtyId",
                table: "TeacherSpecialties",
                column: "SpecialtyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_Subjects_SubjectId1",
                table: "Schedules",
                column: "SubjectId1",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_Subjects_SubjectId1",
                table: "Schedules");

            migrationBuilder.DropTable(
                name: "SubjectRestrictions");

            migrationBuilder.DropTable(
                name: "SubjectSchedules");

            migrationBuilder.DropTable(
                name: "TeacherSpecialties");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_SubjectId1",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SubjectId1",
                table: "Schedules");
        }
    }
}
