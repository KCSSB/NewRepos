using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class CreateResponsiblesForTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponsibleMemberIds",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "ResponsibleForTask",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TaskId = table.Column<int>(type: "integer", nullable: false),
                    MemberOfBoardId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsibleForTask", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResponsibleForTask_MembersOfBoards_MemberOfBoardId",
                        column: x => x.MemberOfBoardId,
                        principalTable: "MembersOfBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ResponsibleForTask_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResponsibleForTask_MemberOfBoardId",
                table: "ResponsibleForTask",
                column: "MemberOfBoardId");

            migrationBuilder.CreateIndex(
                name: "IX_ResponsibleForTask_TaskId",
                table: "ResponsibleForTask",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResponsibleForTask");

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleMemberIds",
                table: "Tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
