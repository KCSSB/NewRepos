using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Task_Card_CardId",
                table: "_Task");

            migrationBuilder.DropForeignKey(
                name: "FK_Board_Group_GroupId",
                table: "Board");

            migrationBuilder.DropForeignKey(
                name: "FK_Card_Board_BoardId",
                table: "Card");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_ProjectUser_LeadId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_Group_Projects_ProjectId",
                table: "Group");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberOfGroup_Group_GroupId",
                table: "MemberOfGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_MemberOfGroup_ProjectUser_ProjectUserId",
                table: "MemberOfGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_Projects_ProjectId",
                table: "ProjectUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUser_Users_UserId",
                table: "ProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUser",
                table: "ProjectUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MemberOfGroup",
                table: "MemberOfGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Group",
                table: "Group");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Card",
                table: "Card");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Board",
                table: "Board");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Task",
                table: "_Task");

            migrationBuilder.RenameTable(
                name: "ProjectUser",
                newName: "ProjectUsers");

            migrationBuilder.RenameTable(
                name: "MemberOfGroup",
                newName: "MembersOfGroups");

            migrationBuilder.RenameTable(
                name: "Group",
                newName: "Groups");

            migrationBuilder.RenameTable(
                name: "Card",
                newName: "Cards");

            migrationBuilder.RenameTable(
                name: "Board",
                newName: "Boards");

            migrationBuilder.RenameTable(
                name: "_Task",
                newName: "Tasks");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUser_UserId_ProjectId",
                table: "ProjectUsers",
                newName: "IX_ProjectUsers_UserId_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUser_ProjectId",
                table: "ProjectUsers",
                newName: "IX_ProjectUsers_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberOfGroup_ProjectUserId",
                table: "MembersOfGroups",
                newName: "IX_MembersOfGroups_ProjectUserId");

            migrationBuilder.RenameIndex(
                name: "IX_MemberOfGroup_GroupId",
                table: "MembersOfGroups",
                newName: "IX_MembersOfGroups_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Group_ProjectId",
                table: "Groups",
                newName: "IX_Groups_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Group_LeadId",
                table: "Groups",
                newName: "IX_Groups_LeadId");

            migrationBuilder.RenameIndex(
                name: "IX_Card_BoardId",
                table: "Cards",
                newName: "IX_Cards_BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Board_GroupId",
                table: "Boards",
                newName: "IX_Boards_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX__Task_CardId",
                table: "Tasks",
                newName: "IX_Tasks_CardId");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Cards",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Progress",
                table: "Cards",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MembersOfGroups",
                table: "MembersOfGroups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cards",
                table: "Cards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Boards",
                table: "Boards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Boards_Groups_GroupId",
                table: "Boards",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Boards_BoardId",
                table: "Cards",
                column: "BoardId",
                principalTable: "Boards",
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

            migrationBuilder.AddForeignKey(
                name: "FK_MembersOfGroups_Groups_GroupId",
                table: "MembersOfGroups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MembersOfGroups_ProjectUsers_ProjectUserId",
                table: "MembersOfGroups",
                column: "ProjectUserId",
                principalTable: "ProjectUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Projects_ProjectId",
                table: "ProjectUsers",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_UserId",
                table: "ProjectUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Cards_CardId",
                table: "Tasks",
                column: "CardId",
                principalTable: "Cards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boards_Groups_GroupId",
                table: "Boards");

            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Boards_BoardId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_ProjectUsers_LeadId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Projects_ProjectId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_MembersOfGroups_Groups_GroupId",
                table: "MembersOfGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_MembersOfGroups_ProjectUsers_ProjectUserId",
                table: "MembersOfGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Projects_ProjectId",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_UserId",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Cards_CardId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectUsers",
                table: "ProjectUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MembersOfGroups",
                table: "MembersOfGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cards",
                table: "Cards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Boards",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "Progress",
                table: "Cards");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "_Task");

            migrationBuilder.RenameTable(
                name: "ProjectUsers",
                newName: "ProjectUser");

            migrationBuilder.RenameTable(
                name: "MembersOfGroups",
                newName: "MemberOfGroup");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "Group");

            migrationBuilder.RenameTable(
                name: "Cards",
                newName: "Card");

            migrationBuilder.RenameTable(
                name: "Boards",
                newName: "Board");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_CardId",
                table: "_Task",
                newName: "IX__Task_CardId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUsers_UserId_ProjectId",
                table: "ProjectUser",
                newName: "IX_ProjectUser_UserId_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectUsers_ProjectId",
                table: "ProjectUser",
                newName: "IX_ProjectUser_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_MembersOfGroups_ProjectUserId",
                table: "MemberOfGroup",
                newName: "IX_MemberOfGroup_ProjectUserId");

            migrationBuilder.RenameIndex(
                name: "IX_MembersOfGroups_GroupId",
                table: "MemberOfGroup",
                newName: "IX_MemberOfGroup_GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_ProjectId",
                table: "Group",
                newName: "IX_Group_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_LeadId",
                table: "Group",
                newName: "IX_Group_LeadId");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_BoardId",
                table: "Card",
                newName: "IX_Card_BoardId");

            migrationBuilder.RenameIndex(
                name: "IX_Boards_GroupId",
                table: "Board",
                newName: "IX_Board_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Task",
                table: "_Task",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectUser",
                table: "ProjectUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MemberOfGroup",
                table: "MemberOfGroup",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Group",
                table: "Group",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Card",
                table: "Card",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Board",
                table: "Board",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__Task_Card_CardId",
                table: "_Task",
                column: "CardId",
                principalTable: "Card",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Board_Group_GroupId",
                table: "Board",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Card_Board_BoardId",
                table: "Card",
                column: "BoardId",
                principalTable: "Board",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_ProjectUser_LeadId",
                table: "Group",
                column: "LeadId",
                principalTable: "ProjectUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Projects_ProjectId",
                table: "Group",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberOfGroup_Group_GroupId",
                table: "MemberOfGroup",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MemberOfGroup_ProjectUser_ProjectUserId",
                table: "MemberOfGroup",
                column: "ProjectUserId",
                principalTable: "ProjectUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_Projects_ProjectId",
                table: "ProjectUser",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUser_Users_UserId",
                table: "ProjectUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
