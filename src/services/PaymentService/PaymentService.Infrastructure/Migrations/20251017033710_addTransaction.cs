using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaymentService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gateway = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubAccount = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AmountIn = table.Column<decimal>(type: "decimal(20,2)", nullable: false, defaultValue: 0m),
                    AmountOut = table.Column<decimal>(type: "decimal(20,2)", nullable: false, defaultValue: 0m),
                    Accumulated = table.Column<decimal>(type: "decimal(20,2)", nullable: false, defaultValue: 0m),
                    Code = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    TransactionContent = table.Column<string>(type: "text", nullable: true),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Body = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
