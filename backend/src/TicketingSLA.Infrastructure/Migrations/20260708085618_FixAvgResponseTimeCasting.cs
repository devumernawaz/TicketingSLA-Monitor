using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSLA.Infrastructure.Migrations
{
    public partial class FixAvgResponseTimeCasting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
ALTER PROCEDURE dbo.GetSLADashboardSummary
    @TenantId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();
    DECLARE @RiskWindow DATETIME2 = DATEADD(HOUR, 1, @Now);
    DECLARE @Yesterday DATETIME2 = DATEADD(HOUR, -24, @Now);

    SELECT
        COUNT(CASE WHEN t.Status <> 'Closed' THEN 1 END) AS OpenTicketCount,

        COUNT(CASE 
            WHEN t.Status <> 'Closed' 
                 AND t.BreachedAt IS NULL 
                 AND t.SlaDeadline <= @RiskWindow 
                 AND t.SlaDeadline > @Now 
            THEN 1 END) AS AtRiskCount,

        COUNT(CASE 
            WHEN t.BreachedAt IS NOT NULL 
            THEN 1 END) AS BreachedCount,

        AVG(CASE 
            WHEN t.Status = 'Closed' 
            THEN CAST(DATEDIFF(MINUTE, t.CreatedAt, t.SlaDeadline) AS FLOAT)
            END) AS AvgResponseTimeMinutes,

        CAST(
            COUNT(CASE WHEN t.BreachedAt >= @Yesterday THEN 1 END) AS FLOAT
        ) 
        / NULLIF(COUNT(CASE WHEN t.CreatedAt >= @Yesterday THEN 1 END), 0) 
        * 100 AS BreachRateLast24HoursPercent

    FROM dbo.Tickets t
    WHERE t.TenantId = @TenantId;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverting to the previous (buggy) version isn't meaningful — 
            // Down() here just leaves the corrected procedure in place.
        }
    }
}