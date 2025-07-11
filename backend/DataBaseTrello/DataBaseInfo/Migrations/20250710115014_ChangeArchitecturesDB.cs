using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeArchitecturesDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Projects_ProjectId",
                table: "Boards");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_ProjectUsers_LeadId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Projects_ProjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_LeadId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_ProjectId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Boards_ProjectId",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Boards");

            migrationBuilder.AddColumn<string>(
                name: "projectRole",
                table: "ProjectUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GroupRole",
                table: "MembersOfGroups",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "projectRole",
                table: "ProjectUsers");

            migrationBuilder.DropColumn(
                name: "GroupRole",
                table: "MembersOfGroups");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "BoardId",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectId",
                table: "Boards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LeadId",
                table: "Groups",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ProjectId",
                table: "Groups",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_ProjectId",
                table: "Boards",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Projects_ProjectId",
                table: "Boards",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_ProjectUsers_LeadId",
                table: "Groups",
                column: "LeadId",
                principalTable: "ProjectUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Projects_ProjectId",
                table: "Groups",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
