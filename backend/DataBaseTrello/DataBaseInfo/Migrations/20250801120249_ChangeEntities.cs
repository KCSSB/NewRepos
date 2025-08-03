using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseInfo.Migrations
{
    /// <inheritdoc />
    public partial class ChangeEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Cards");

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfDeadline",
                table: "Tasks",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tasks",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "ResponsibleForCard",
                table: "Tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateOfDeadline",
                table: "Projects",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "DateStartWork",
                table: "Projects",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfDeadline",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "ResponsibleForCard",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DateOfDeadline",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "DateStartWork",
                table: "Projects");

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Cards",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
