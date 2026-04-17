using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliverWholesale.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots");

            migrationBuilder.DropColumn(
                name: "Fournisseur",
                table: "StockLots");

            migrationBuilder.DropColumn(
                name: "PrixAchatLot",
                table: "StockLots");

            migrationBuilder.DropColumn(
                name: "Unite",
                table: "StockLots");

            migrationBuilder.DropColumn(
                name: "PrixAchat",
                table: "Produits");

            migrationBuilder.RenameColumn(
                name: "QuantiteAchetee",
                table: "StockLots",
                newName: "QuantiteRestante");

            migrationBuilder.RenameColumn(
                name: "DateAchat",
                table: "StockLots",
                newName: "DateReception");

            migrationBuilder.RenameColumn(
                name: "PrixVente",
                table: "Produits",
                newName: "Prix");

            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "ProduitId",
                table: "StockLots",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "AchatLotId",
                table: "StockLots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SeuilAlerte",
                table: "Produits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLivraisonPrevue",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AchatLots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProduitId = table.Column<int>(type: "int", nullable: false),
                    DateAchat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantiteAchetee = table.Column<int>(type: "int", nullable: false),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fournisseur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroLot = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AchatLots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AchatLots_Produits_ProduitId",
                        column: x => x.ProduitId,
                        principalTable: "Produits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LotCommandes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockLotId = table.Column<int>(type: "int", nullable: false),
                    OrderDetailId = table.Column<int>(type: "int", nullable: false),
                    QuantitePrelevee = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LotCommandes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LotCommandes_OrderDetails_OrderDetailId",
                        column: x => x.OrderDetailId,
                        principalTable: "OrderDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LotCommandes_StockLots_StockLotId",
                        column: x => x.StockLotId,
                        principalTable: "StockLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockLots_AchatLotId",
                table: "StockLots",
                column: "AchatLotId");

            migrationBuilder.CreateIndex(
                name: "IX_AchatLots_NumeroLot",
                table: "AchatLots",
                column: "NumeroLot",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AchatLots_ProduitId",
                table: "AchatLots",
                column: "ProduitId");

            migrationBuilder.CreateIndex(
                name: "IX_LotCommandes_OrderDetailId",
                table: "LotCommandes",
                column: "OrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_LotCommandes_StockLotId",
                table: "LotCommandes",
                column: "StockLotId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockLots_AchatLots_AchatLotId",
                table: "StockLots",
                column: "AchatLotId",
                principalTable: "AchatLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockLots_AchatLots_AchatLotId",
                table: "StockLots");

            migrationBuilder.DropForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots");

            migrationBuilder.DropTable(
                name: "AchatLots");

            migrationBuilder.DropTable(
                name: "LotCommandes");

            migrationBuilder.DropIndex(
                name: "IX_StockLots_AchatLotId",
                table: "StockLots");

            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AchatLotId",
                table: "StockLots");

            migrationBuilder.DropColumn(
                name: "SeuilAlerte",
                table: "Produits");

            migrationBuilder.RenameColumn(
                name: "QuantiteRestante",
                table: "StockLots",
                newName: "QuantiteAchetee");

            migrationBuilder.RenameColumn(
                name: "DateReception",
                table: "StockLots",
                newName: "DateAchat");

            migrationBuilder.RenameColumn(
                name: "Prix",
                table: "Produits",
                newName: "PrixVente");

            migrationBuilder.AlterColumn<int>(
                name: "ProduitId",
                table: "StockLots",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Fournisseur",
                table: "StockLots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrixAchatLot",
                table: "StockLots",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unite",
                table: "StockLots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PrixAchat",
                table: "Produits",
                type: "decimal(18,3)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateLivraisonPrevue",
                table: "Deliveries",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddForeignKey(
                name: "FK_StockLots_Produits_ProduitId",
                table: "StockLots",
                column: "ProduitId",
                principalTable: "Produits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
