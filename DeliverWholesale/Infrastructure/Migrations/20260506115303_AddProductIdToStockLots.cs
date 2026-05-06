using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliverWholesale.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIdToStockLots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots");

            migrationBuilder.AlterColumn<int>(
                name: "ProduitId",
                table: "StockLots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots");

            migrationBuilder.AlterColumn<int>(
                name: "ProduitId",
                table: "StockLots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id");
        }
    }
}
