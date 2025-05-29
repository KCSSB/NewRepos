using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableHashedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passwords_Users_UserId",
                table: "Passwords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Passwords",
                table: "Passwords");

            migrationBuilder.DropIndex(
                name: "IX_Passwords_UserId",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Passwords");

            migrationBuilder.RenameColumn(
                name: "password",
                table: "Passwords",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Passwords",
                newName: "userId");

            migrationBuilder.AddColumn<byte[]>(
                name: "storedHash",
                table: "Passwords",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "storedSalt",
                table: "Passwords",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Passwords",
                table: "Passwords",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Passwords_Users_userId",
                table: "Passwords",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Passwords_Users_userId",
                table: "Passwords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Passwords",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "storedHash",
                table: "Passwords");

            migrationBuilder.DropColumn(
                name: "storedSalt",
                table: "Passwords");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Passwords",
                newName: "password");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Passwords",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Passwords",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Passwords",
                table: "Passwords",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Passwords_UserId",
                table: "Passwords",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Passwords_Users_UserId",
                table: "Passwords",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
