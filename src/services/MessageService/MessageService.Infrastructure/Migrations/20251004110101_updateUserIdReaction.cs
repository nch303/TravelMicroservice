using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateUserIdReaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_ChatParticipants_UserId",
                table: "MessageReactions");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "MessageReactions",
                newName: "ParticipantId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageReactions_UserId_MessageId",
                table: "MessageReactions",
                newName: "IX_MessageReactions_ParticipantId_MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_ChatParticipants_ParticipantId",
                table: "MessageReactions",
                column: "ParticipantId",
                principalTable: "ChatParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageReactions_ChatParticipants_ParticipantId",
                table: "MessageReactions");

            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "MessageReactions",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_MessageReactions_ParticipantId_MessageId",
                table: "MessageReactions",
                newName: "IX_MessageReactions_UserId_MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageReactions_ChatParticipants_UserId",
                table: "MessageReactions",
                column: "UserId",
                principalTable: "ChatParticipants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
