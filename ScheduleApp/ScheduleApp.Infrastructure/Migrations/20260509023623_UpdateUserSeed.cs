using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$voq.ILCj3M49oj7lMN9l2.Tj9U56jcyrVUf12TxhFqlRydeKe99tS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PasswordHash",
                value: "$2a$11$ZSBnN5nRMHGs9JFyWGfFNeaShips2b7SIHomyDQRjTu2wFJJMPMnS");
        }
    }
}
