using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DeliverWholesale.Migrations
{
    /// <inheritdoc />
    public partial class AddResolvedByToReclamation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResolvedByUserId",
                table: "Reclamations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reclamations_ResolvedByUserId",
                table: "Reclamations",
                column: "ResolvedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reclamations_Users_ResolvedByUserId",
                table: "Reclamations",
                column: "ResolvedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reclamations_Users_ResolvedByUserId",
                table: "Reclamations");

            migrationBuilder.DropIndex(
                name: "IX_Reclamations_ResolvedByUserId",
                table: "Reclamations");

            migrationBuilder.DropColumn(
                name: "ResolvedByUserId",
                table: "Reclamations");
        }
    }
}
