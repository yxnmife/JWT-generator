using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleanAuthenticator.Api.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountsTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountsTable", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StockTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Div = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Purchase = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MarketCap = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTable", x => x.Id);
                });

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

            migrationBuilder.DropTable(
                name: "AccountsTable");

            migrationBuilder.DropTable(
                name: "StockTable");
        }
    }
}
