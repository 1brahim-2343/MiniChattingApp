using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniChattingApp.Migrations
{
    /// <inheritdoc />
    public partial class MessageDateTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SentTime",
                table: "Messages",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SentTime",
                table: "Messages");
        }
    }
}
