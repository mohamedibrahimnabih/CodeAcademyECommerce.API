using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAcademyECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLoggingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAT",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAT",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAT",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAT",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAT",
                table: "Brands",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAT",
                table: "Brands",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UpdatedAT",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreateAT",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAT",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreateAT",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "UpdatedAT",
                table: "Brands");
        }
    }
}
