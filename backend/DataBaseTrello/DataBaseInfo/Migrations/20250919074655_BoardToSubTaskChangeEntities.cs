using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class BoardToSubTaskChangeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Complete",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Cards");

            migrationBuilder.RenameColumn(
                name: "TaskResponsibleMembersц",
                table: "Tasks",
                newName: "ResponsibleMemberIds");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ResponsibleMemberIds",
                table: "Tasks",
                newName: "TaskResponsibleMembersц");

            migrationBuilder.AddColumn<bool>(
                name: "Complete",
                table: "Tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
