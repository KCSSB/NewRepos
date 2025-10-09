using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class AddResponsiblesForTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibleForTask_MembersOfBoards_MemberOfBoardId",
                table: "ResponsibleForTask");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibleForTask_Tasks_TaskId",
                table: "ResponsibleForTask");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponsibleForTask",
                table: "ResponsibleForTask");

            migrationBuilder.RenameTable(
                name: "ResponsibleForTask",
                newName: "ResponsibleForTasks");

            migrationBuilder.RenameIndex(
                name: "IX_ResponsibleForTask_TaskId",
                table: "ResponsibleForTasks",
                newName: "IX_ResponsibleForTasks_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_ResponsibleForTask_MemberOfBoardId",
                table: "ResponsibleForTasks",
                newName: "IX_ResponsibleForTasks_MemberOfBoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponsibleForTasks",
                table: "ResponsibleForTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibleForTasks_MembersOfBoards_MemberOfBoardId",
                table: "ResponsibleForTasks",
                column: "MemberOfBoardId",
                principalTable: "MembersOfBoards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibleForTasks_Tasks_TaskId",
                table: "ResponsibleForTasks",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibleForTasks_MembersOfBoards_MemberOfBoardId",
                table: "ResponsibleForTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibleForTasks_Tasks_TaskId",
                table: "ResponsibleForTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponsibleForTasks",
                table: "ResponsibleForTasks");

            migrationBuilder.RenameTable(
                name: "ResponsibleForTasks",
                newName: "ResponsibleForTask");

            migrationBuilder.RenameIndex(
                name: "IX_ResponsibleForTasks_TaskId",
                table: "ResponsibleForTask",
                newName: "IX_ResponsibleForTask_TaskId");

            migrationBuilder.RenameIndex(
                name: "IX_ResponsibleForTasks_MemberOfBoardId",
                table: "ResponsibleForTask",
                newName: "IX_ResponsibleForTask_MemberOfBoardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponsibleForTask",
                table: "ResponsibleForTask",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibleForTask_MembersOfBoards_MemberOfBoardId",
                table: "ResponsibleForTask",
                column: "MemberOfBoardId",
                principalTable: "MembersOfBoards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibleForTask_Tasks_TaskId",
                table: "ResponsibleForTask",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
