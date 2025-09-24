using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CategoryService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePhotoCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoCategories");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Categories_CategoryGuid",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Categories_CategoryGuid",
                table: "Categories",
                column: "CategoryGuid");

            migrationBuilder.CreateTable(
                name: "PhotoCategories",
                columns: table => new
                {
                    PhotoGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoCategories", x => new { x.PhotoGuid, x.CategoryGuid });
                    table.ForeignKey(
                        name: "FK_PhotoCategories_Categories_CategoryGuid",
                        column: x => x.CategoryGuid,
                        principalTable: "Categories",
                        principalColumn: "CategoryGuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoCategories_CategoryGuid",
                table: "PhotoCategories",
                column: "CategoryGuid");
        }
    }
}
