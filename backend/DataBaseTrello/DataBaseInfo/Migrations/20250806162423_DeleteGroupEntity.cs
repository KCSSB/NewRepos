using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class DeleteGroupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Groups_GroupId",
                table: "Boards");

            migrationBuilder.DropTable(
                name: "MembersOfGroups");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Boards_GroupId",
                table: "Boards");

            migrationBuilder.RenameColumn(
                name: "projectRole",
                table: "ProjectUsers",
                newName: "ProjectRole");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Boards",
                newName: "LeadOfBoardId");

            migrationBuilder.CreateTable(
                name: "MembersOfBoards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BoardRole = table.Column<string>(type: "text", nullable: false),
                    ProjectUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    BoardId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersOfBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembersOfBoards_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembersOfBoards_ProjectUsers_ProjectUserId",
                        column: x => x.ProjectUserId,
                        principalTable: "ProjectUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembersOfBoards_BoardId",
                table: "MembersOfBoards",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersOfBoards_ProjectUserId",
                table: "MembersOfBoards",
                column: "ProjectUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MembersOfBoards");

            migrationBuilder.RenameColumn(
                name: "ProjectRole",
                table: "ProjectUsers",
                newName: "projectRole");

            migrationBuilder.RenameColumn(
                name: "LeadOfBoardId",
                table: "Boards",
                newName: "GroupId");

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LeadId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MembersOfGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupRole = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembersOfGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MembersOfGroups_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MembersOfGroups_ProjectUsers_ProjectUserId",
                        column: x => x.ProjectUserId,
                        principalTable: "ProjectUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_GroupId",
                table: "Boards",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersOfGroups_GroupId",
                table: "MembersOfGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MembersOfGroups_ProjectUserId",
                table: "MembersOfGroups",
                column: "ProjectUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Groups_GroupId",
                table: "Boards",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
