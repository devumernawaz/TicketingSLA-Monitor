using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSLA.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketDashboardIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TenantId_Status_SlaDeadline",
                table: "Tickets",
                columns: new[] { "TenantId", "Status", "SlaDeadline" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_TenantId_Status_SlaDeadline",
                table: "Tickets");
        }
    }
}
