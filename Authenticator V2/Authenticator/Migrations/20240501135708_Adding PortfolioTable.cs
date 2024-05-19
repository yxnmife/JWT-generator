using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Authenticator.Migrations
{
    public partial class AddingPortfolioTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Porttfolios",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    StockId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Porttfolios", x => new { x.AccountId, x.StockId });
                    table.ForeignKey(
                        name: "FK_Porttfolios_AccountsTable_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountsTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Porttfolios_StockTable_StockId",
                        column: x => x.StockId,
                        principalTable: "StockTable",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Porttfolios_StockId",
                table: "Porttfolios",
                column: "StockId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Porttfolios");
        }
    }
}
