using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FluentChat.Migrations
{
    /// <inheritdoc />
    public partial class ChatSessionAddTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Service",
                table: "ChatSessions",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "PromptSettings",
                table: "ChatSessions",
                type: "jsonb",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ChatSessions",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PromptSettings",
                table: "ChatSessions");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ChatSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Service",
                table: "ChatSessions",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(60)",
                oldMaxLength: 60);
        }
    }
}
