using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateMedia_addParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParticipantId",
                table: "ScheduleMedias",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleMedias_ParticipantId",
                table: "ScheduleMedias",
                column: "ParticipantId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMedias_ScheduleParticipants_ParticipantId",
                table: "ScheduleMedias",
                column: "ParticipantId",
                principalTable: "ScheduleParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMedias_ScheduleParticipants_ParticipantId",
                table: "ScheduleMedias");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleMedias_ParticipantId",
                table: "ScheduleMedias");

            migrationBuilder.DropColumn(
                name: "ParticipantId",
                table: "ScheduleMedias");
        }
    }
}
