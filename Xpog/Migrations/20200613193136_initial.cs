using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Xpog.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(nullable: false),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrentExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: true),
                    ExpenseDataId = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CurrentExpenses_ExpenseDatas_ExpenseDataId",
                        column: x => x.ExpenseDataId,
                        principalTable: "ExpenseDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrentExpenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: true),
                    ExpenseDataId = table.Column<int>(nullable: true),
                    TriggeringDateOfMonth = table.Column<int>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyExpenses_ExpenseDatas_ExpenseDataId",
                        column: x => x.ExpenseDataId,
                        principalTable: "ExpenseDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyExpenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RepeatableExpenses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: true),
                    ExpenseDataId = table.Column<int>(nullable: true),
                    timeToRepeatInDays = table.Column<int>(nullable: false),
                    daysToTrigger = table.Column<int>(nullable: false),
                    ExpiryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepeatableExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RepeatableExpenses_ExpenseDatas_ExpenseDataId",
                        column: x => x.ExpenseDataId,
                        principalTable: "ExpenseDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RepeatableExpenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentExpenses_ExpenseDataId",
                table: "CurrentExpenses",
                column: "ExpenseDataId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentExpenses_UserId",
                table: "CurrentExpenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyExpenses_ExpenseDataId",
                table: "MonthlyExpenses",
                column: "ExpenseDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyExpenses_UserId",
                table: "MonthlyExpenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatableExpenses_ExpenseDataId",
                table: "RepeatableExpenses",
                column: "ExpenseDataId");

            migrationBuilder.CreateIndex(
                name: "IX_RepeatableExpenses_UserId",
                table: "RepeatableExpenses",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentExpenses");

            migrationBuilder.DropTable(
                name: "MonthlyExpenses");

            migrationBuilder.DropTable(
                name: "RepeatableExpenses");

            migrationBuilder.DropTable(
                name: "ExpenseDatas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
