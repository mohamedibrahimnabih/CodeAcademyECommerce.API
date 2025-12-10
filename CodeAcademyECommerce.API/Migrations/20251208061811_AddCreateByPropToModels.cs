using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAcademyECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddCreateByPropToModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreateById",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateById",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreateById",
                table: "Brands",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreateById",
                table: "Products",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CreateById",
                table: "Categories",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_CreateById",
                table: "Brands",
                column: "CreateById");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_AspNetUsers_CreateById",
                table: "Brands",
                column: "CreateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_AspNetUsers_CreateById",
                table: "Categories",
                column: "CreateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_CreateById",
                table: "Products",
                column: "CreateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_AspNetUsers_CreateById",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_AspNetUsers_CreateById",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_CreateById",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CreateById",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CreateById",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Brands_CreateById",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "CreateById",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreateById",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreateById",
                table: "Brands");
        }
    }
}
