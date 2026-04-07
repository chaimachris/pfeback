using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliverWholesale.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStockAndOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotesLivraison",
                table: "Deliveries");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NotesLivraison",
                table: "Deliveries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
