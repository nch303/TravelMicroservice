using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Latitude : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "ScheduleActivities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "ScheduleActivities",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ScheduleActivities");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ScheduleActivities");
        }
    }
}
