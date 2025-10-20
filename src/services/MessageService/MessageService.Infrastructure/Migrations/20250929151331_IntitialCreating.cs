using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntitialCreating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    JoinedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastSeenAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ChatGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatParticipants_ChatGroups_ChatGroupId",
                        column: x => x.ChatGroupId,
                        principalTable: "ChatGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EditAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "ChatGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatMessages_ParentMessageId",
                        column: x => x.ParentMessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatParticipants_SenderId",
                        column: x => x.SenderId,
                        principalTable: "ChatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageReactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReactionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReactions_ChatMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReactions_ChatParticipants_UserId",
                        column: x => x.UserId,
                        principalTable: "ChatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MessageReads",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ReaderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageReads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageReads_ChatMessages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "ChatMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageReads_ChatParticipants_ReaderId",
                        column: x => x.ReaderId,
                        principalTable: "ChatParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_GroupId",
                table: "ChatMessages",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ParentMessageId",
                table: "ChatMessages",
                column: "ParentMessageId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_SenderId",
                table: "ChatMessages",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatParticipants_ChatGroupId",
                table: "ChatParticipants",
                column: "ChatGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_MessageId",
                table: "MessageReactions",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReactions_UserId_MessageId",
                table: "MessageReactions",
                columns: new[] { "UserId", "MessageId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_MessageId",
                table: "MessageReads",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageReads_ReaderId",
                table: "MessageReads",
                column: "ReaderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageReactions");

            migrationBuilder.DropTable(
                name: "MessageReads");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "ChatParticipants");

            migrationBuilder.DropTable(
                name: "ChatGroups");
        }
    }
}
