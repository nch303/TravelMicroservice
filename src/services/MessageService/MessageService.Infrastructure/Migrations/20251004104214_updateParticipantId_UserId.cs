using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateParticipantId_UserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "ChatParticipants",
                newName: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ChatParticipants",
                newName: "ParticipantId");
        }
    }
}
