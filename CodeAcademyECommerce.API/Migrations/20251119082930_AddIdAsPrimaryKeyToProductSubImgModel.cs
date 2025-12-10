using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAcademyECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddIdAsPrimaryKeyToProductSubImgModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSubImgs",
                table: "ProductSubImgs");

            migrationBuilder.AlterColumn<string>(
                name: "SubImg",
                table: "ProductSubImgs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ProductSubImgs",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSubImgs",
                table: "ProductSubImgs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSubImgs",
                table: "ProductSubImgs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ProductSubImgs");

            migrationBuilder.AlterColumn<string>(
                name: "SubImg",
                table: "ProductSubImgs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSubImgs",
                table: "ProductSubImgs",
                columns: new[] { "SubImg", "ProductId" });
        }
    }
}
