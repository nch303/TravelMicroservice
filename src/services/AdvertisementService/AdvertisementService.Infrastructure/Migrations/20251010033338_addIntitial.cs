using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdvertisementService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addIntitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdvertisementPackages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    MaxPostCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementPackages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PartnerPackagePurchases",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RemainingPostCount = table.Column<int>(type: "int", nullable: false),
                    PaymentTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PackageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerPackagePurchases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartnerPackagePurchases_AdvertisementPackages_PackageId",
                        column: x => x.PackageId,
                        principalTable: "AdvertisementPackages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PostedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    PackagePurchaseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementPosts_PartnerPackagePurchases_PackagePurchaseId",
                        column: x => x.PackagePurchaseId,
                        principalTable: "PartnerPackagePurchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementMedias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MediaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AdvertisementPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdvertisementMedias_AdvertisementPosts_AdvertisementPostId",
                        column: x => x.AdvertisementPostId,
                        principalTable: "AdvertisementPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementMedias_AdvertisementPostId",
                table: "AdvertisementMedias",
                column: "AdvertisementPostId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPosts_PackagePurchaseId",
                table: "AdvertisementPosts",
                column: "PackagePurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementPosts_PartnerId",
                table: "AdvertisementPosts",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPackagePurchases_PackageId",
                table: "PartnerPackagePurchases",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerPackagePurchases_PartnerId",
                table: "PartnerPackagePurchases",
                column: "PartnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementMedias");

            migrationBuilder.DropTable(
                name: "AdvertisementPosts");

            migrationBuilder.DropTable(
                name: "PartnerPackagePurchases");

            migrationBuilder.DropTable(
                name: "AdvertisementPackages");
        }
    }
}
