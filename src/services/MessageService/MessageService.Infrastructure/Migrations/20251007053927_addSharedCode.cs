using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSharedCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SharedCode",
                table: "ChatGroups",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SharedExpired",
                table: "ChatGroups",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatGroups_SharedCode",
                table: "ChatGroups",
                column: "SharedCode",
                unique: true,
                filter: "[SharedCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ChatGroups_SharedCode",
                table: "ChatGroups");

            migrationBuilder.DropColumn(
                name: "SharedCode",
                table: "ChatGroups");

            migrationBuilder.DropColumn(
                name: "SharedExpired",
                table: "ChatGroups");
        }
    }
}
