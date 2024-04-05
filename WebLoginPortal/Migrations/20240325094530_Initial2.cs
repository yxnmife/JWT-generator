using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebLoginPortal.Migrations
{
    /// <inheritdoc />
    public partial class Initial2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordHash",
                table: "UsersTable",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "PasswordSalt",
                table: "UsersTable",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "UsersTable");

            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                table: "UsersTable");

           
        }
    }
}
