using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIsDeletedReaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MessageReactions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MessageReactions");
        }
    }
}
