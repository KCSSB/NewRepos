using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class RefreshHashpasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.AddColumn<string>(
                name: "UserPassword",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPassword",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    userId = table.Column<int>(type: "integer", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    storedHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    storedSalt = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.userId);
                    table.ForeignKey(
                        name: "FK_Passwords_Users_userId",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }
    }
}
