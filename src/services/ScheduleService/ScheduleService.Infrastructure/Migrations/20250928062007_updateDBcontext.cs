using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScheduleService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateDBcontext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckedItem_Schedules_ScheduleId",
                table: "CheckedItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleActivity_Schedules_ScheduleId",
                table: "ScheduleActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMedia_Schedules_ScheduleId",
                table: "ScheduleMedia");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleParticipant_Schedules_ScheduleId",
                table: "ScheduleParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleParticipant",
                table: "ScheduleParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleMedia",
                table: "ScheduleMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleActivity",
                table: "ScheduleActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckedItemUser",
                table: "CheckedItemUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckedItem",
                table: "CheckedItem");

            migrationBuilder.RenameTable(
                name: "ScheduleParticipant",
                newName: "ScheduleParticipants");

            migrationBuilder.RenameTable(
                name: "ScheduleMedia",
                newName: "ScheduleMedias");

            migrationBuilder.RenameTable(
                name: "ScheduleActivity",
                newName: "ScheduleActivities");

            migrationBuilder.RenameTable(
                name: "CheckedItemUser",
                newName: "CheckedItemUsers");

            migrationBuilder.RenameTable(
                name: "CheckedItem",
                newName: "CheckLists");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleParticipant_ScheduleId",
                table: "ScheduleParticipants",
                newName: "IX_ScheduleParticipants_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleMedia_ScheduleId",
                table: "ScheduleMedias",
                newName: "IX_ScheduleMedias_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleActivity_ScheduleId",
                table: "ScheduleActivities",
                newName: "IX_ScheduleActivities_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckedItem_ScheduleId",
                table: "CheckLists",
                newName: "IX_CheckLists_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleParticipants",
                table: "ScheduleParticipants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleMedias",
                table: "ScheduleMedias",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleActivities",
                table: "ScheduleActivities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckedItemUsers",
                table: "CheckedItemUsers",
                column: "CheckedItemId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckLists",
                table: "CheckLists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckLists_Schedules_ScheduleId",
                table: "CheckLists",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleActivities_Schedules_ScheduleId",
                table: "ScheduleActivities",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMedias_Schedules_ScheduleId",
                table: "ScheduleMedias",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleParticipants_Schedules_ScheduleId",
                table: "ScheduleParticipants",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckLists_Schedules_ScheduleId",
                table: "CheckLists");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleActivities_Schedules_ScheduleId",
                table: "ScheduleActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleMedias_Schedules_ScheduleId",
                table: "ScheduleMedias");

            migrationBuilder.DropForeignKey(
                name: "FK_ScheduleParticipants_Schedules_ScheduleId",
                table: "ScheduleParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleParticipants",
                table: "ScheduleParticipants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleMedias",
                table: "ScheduleMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleActivities",
                table: "ScheduleActivities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckLists",
                table: "CheckLists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckedItemUsers",
                table: "CheckedItemUsers");

            migrationBuilder.RenameTable(
                name: "ScheduleParticipants",
                newName: "ScheduleParticipant");

            migrationBuilder.RenameTable(
                name: "ScheduleMedias",
                newName: "ScheduleMedia");

            migrationBuilder.RenameTable(
                name: "ScheduleActivities",
                newName: "ScheduleActivity");

            migrationBuilder.RenameTable(
                name: "CheckLists",
                newName: "CheckedItem");

            migrationBuilder.RenameTable(
                name: "CheckedItemUsers",
                newName: "CheckedItemUser");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleParticipants_ScheduleId",
                table: "ScheduleParticipant",
                newName: "IX_ScheduleParticipant_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleMedias_ScheduleId",
                table: "ScheduleMedia",
                newName: "IX_ScheduleMedia_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_ScheduleActivities_ScheduleId",
                table: "ScheduleActivity",
                newName: "IX_ScheduleActivity_ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_CheckLists_ScheduleId",
                table: "CheckedItem",
                newName: "IX_CheckedItem_ScheduleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleParticipant",
                table: "ScheduleParticipant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleMedia",
                table: "ScheduleMedia",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleActivity",
                table: "ScheduleActivity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckedItem",
                table: "CheckedItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckedItemUser",
                table: "CheckedItemUser",
                column: "CheckedItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckedItem_Schedules_ScheduleId",
                table: "CheckedItem",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleActivity_Schedules_ScheduleId",
                table: "ScheduleActivity",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleMedia_Schedules_ScheduleId",
                table: "ScheduleMedia",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScheduleParticipant_Schedules_ScheduleId",
                table: "ScheduleParticipant",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
