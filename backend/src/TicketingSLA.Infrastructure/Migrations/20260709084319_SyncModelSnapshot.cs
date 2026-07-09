using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSLA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SLADailyTrends",
                columns: table => new
                {
                    TrendDate = table.Column<DateOnly>(type: "date", nullable: false),
                    TicketsCreatedCount = table.Column<int>(type: "int", nullable: false),
                    BreachedCount = table.Column<int>(type: "int", nullable: false),
                    BreachRatePercent = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SLADailyTrends");
        }
    }
}
