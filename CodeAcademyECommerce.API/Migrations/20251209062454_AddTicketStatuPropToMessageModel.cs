using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CodeAcademyECommerce.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketStatuPropToMessageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TicketStatus",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketStatus",
                table: "Messages");
        }
    }
}
