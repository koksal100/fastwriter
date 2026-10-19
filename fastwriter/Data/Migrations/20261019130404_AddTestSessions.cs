using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fastwriter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTestSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    TypingTextId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestSessions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestSessions_TypingTexts_TypingTextId",
                        column: x => x.TypingTextId,
                        principalTable: "TypingTexts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestSessions_StartedAtUtc",
                table: "TestSessions",
                column: "StartedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessions_TypingTextId",
                table: "TestSessions",
                column: "TypingTextId");

            migrationBuilder.CreateIndex(
                name: "IX_TestSessions_UserId_CompletedAtUtc",
                table: "TestSessions",
                columns: new[] { "UserId", "CompletedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestSessions");
        }
    }
}
