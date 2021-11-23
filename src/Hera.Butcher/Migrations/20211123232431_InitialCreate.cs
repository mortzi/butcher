using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hera.Butcher.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    Market = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryPrice = table.Column<decimal>(type: "TEXT", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    StopLoss = table.Column<decimal>(type: "TEXT", nullable: false),
                    RiskRewardRatio = table.Column<decimal>(type: "TEXT", nullable: false),
                    TakeProfit = table.Column<decimal>(type: "TEXT", nullable: false),
                    Fee = table.Column<decimal>(type: "TEXT", nullable: false),
                    Result = table.Column<int>(type: "INTEGER", nullable: false),
                    NetProfit = table.Column<decimal>(type: "TEXT", nullable: false),
                    AccountGrowth = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TelegramId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trades");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
