using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class FixEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemberResponsibleForCard",
                table: "Tasks",
                newName: "TaskResponsibleMembersц");

            migrationBuilder.RenameColumn(
                name: "DateStartWork",
                table: "Tasks",
                newName: "DateOfStartWork");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskResponsibleMembersц",
                table: "Tasks",
                newName: "MemberResponsibleForCard");

            migrationBuilder.RenameColumn(
                name: "DateOfStartWork",
                table: "Tasks",
                newName: "DateStartWork");
        }
    }
}
