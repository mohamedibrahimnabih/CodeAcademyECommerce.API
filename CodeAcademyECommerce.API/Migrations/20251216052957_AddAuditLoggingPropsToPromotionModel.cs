using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAcademyECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLoggingPropsToPromotionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CreateAt",
                table: "Promotions",
                newName: "CreateAT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateAT",
                table: "Promotions",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAtTime",
                table: "Promotions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreateById",
                table: "Promotions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAT",
                table: "Promotions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Promotions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_CreateById",
                table: "Promotions",
                column: "CreateById");

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_UpdatedById",
                table: "Promotions",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_AspNetUsers_CreateById",
                table: "Promotions",
                column: "CreateById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_AspNetUsers_UpdatedById",
                table: "Promotions",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_AspNetUsers_CreateById",
                table: "Promotions");

            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_AspNetUsers_UpdatedById",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_CreateById",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_UpdatedById",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "CreateAtTime",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "CreateById",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "UpdatedAT",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Promotions");

            migrationBuilder.RenameColumn(
                name: "CreateAT",
                table: "Promotions",
                newName: "CreateAt");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateAt",
                table: "Promotions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
