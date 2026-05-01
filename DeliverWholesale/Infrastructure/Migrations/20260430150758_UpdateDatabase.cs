using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliverWholesale.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeuilAlerte",
                table: "Produits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SeuilAlerte",
                table: "Produits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
