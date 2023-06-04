using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskHive.Infrastructure.Persistence.Migrations
{
    public partial class Removedunusedtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IssueHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IssueHistory",
                columns: table => new
                {
                    IssueHistoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IssueId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IssueHistory", x => x.IssueHistoryId);
                    table.ForeignKey(
                        name: "FK_IssueHistory_Account_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Account",
                        principalColumn: "AccountId");
                    table.ForeignKey(
                        name: "FK_IssueHistory_Issue_IssueId",
                        column: x => x.IssueId,
                        principalTable: "Issue",
                        principalColumn: "IssueId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistory_AccountId",
                table: "IssueHistory",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_IssueHistory_IssueId",
                table: "IssueHistory",
                column: "IssueId");
        }
    }
}
