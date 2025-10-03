using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedia : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMedias_Schedules_ScheduleId",
                table: "ScheduleMedias");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "ScheduleMedias",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<int>(
                name: "ActivityId",
                table: "ScheduleMedias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleMedias_ActivityId",
                table: "ScheduleMedias",
                column: "ActivityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMedias_ScheduleActivities_ActivityId",
                table: "ScheduleMedias",
                column: "ActivityId",
                principalTable: "ScheduleActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMedias_Schedules_ScheduleId",
                table: "ScheduleMedias",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMedias_ScheduleActivities_ActivityId",
                table: "ScheduleMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMedias_Schedules_ScheduleId",
                table: "ScheduleMedias");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleMedias_ActivityId",
                table: "ScheduleMedias");

            migrationBuilder.DropColumn(
                name: "ActivityId",
                table: "ScheduleMedias");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "ScheduleMedias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMedias_Schedules_ScheduleId",
                table: "ScheduleMedias",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
