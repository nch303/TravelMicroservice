using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Attendance2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityAttendance_ScheduleActivities_ActivityId",
                table: "ActivityAttendance");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityAttendance_ScheduleParticipants_ParticipantId",
                table: "ActivityAttendance");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityAttendance",
                table: "ActivityAttendance");

            migrationBuilder.RenameTable(
                name: "ActivityAttendance",
                newName: "ActivityAttendances");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityAttendance_ParticipantId",
                table: "ActivityAttendances",
                newName: "IX_ActivityAttendances_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityAttendance_ActivityId",
                table: "ActivityAttendances",
                newName: "IX_ActivityAttendances_ActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityAttendances",
                table: "ActivityAttendances",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityAttendances_ScheduleActivities_ActivityId",
                table: "ActivityAttendances",
                column: "ActivityId",
                principalTable: "ScheduleActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityAttendances_ScheduleParticipants_ParticipantId",
                table: "ActivityAttendances",
                column: "ParticipantId",
                principalTable: "ScheduleParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActivityAttendances_ScheduleActivities_ActivityId",
                table: "ActivityAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_ActivityAttendances_ScheduleParticipants_ParticipantId",
                table: "ActivityAttendances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ActivityAttendances",
                table: "ActivityAttendances");

            migrationBuilder.RenameTable(
                name: "ActivityAttendances",
                newName: "ActivityAttendance");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityAttendances_ParticipantId",
                table: "ActivityAttendance",
                newName: "IX_ActivityAttendance_ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_ActivityAttendances_ActivityId",
                table: "ActivityAttendance",
                newName: "IX_ActivityAttendance_ActivityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ActivityAttendance",
                table: "ActivityAttendance",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityAttendance_ScheduleActivities_ActivityId",
                table: "ActivityAttendance",
                column: "ActivityId",
                principalTable: "ScheduleActivities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ActivityAttendance_ScheduleParticipants_ParticipantId",
                table: "ActivityAttendance",
                column: "ParticipantId",
                principalTable: "ScheduleParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
