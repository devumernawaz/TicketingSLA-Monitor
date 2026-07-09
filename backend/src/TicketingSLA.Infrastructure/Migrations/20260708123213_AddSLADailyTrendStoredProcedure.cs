using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSLA.Infrastructure.Migrations
{
    public partial class AddSLADailyTrendStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE dbo.GetSLADailyTrend
    @TenantId UNIQUEIDENTIFIER,
    @DaysBack INT = 14
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH DateSeries AS (
        SELECT CAST(DATEADD(DAY, -n, CAST(SYSUTCDATETIME() AS DATE)) AS DATE) AS TrendDate
        FROM (
            SELECT TOP (@DaysBack) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS n
            FROM sys.objects
        ) AS Numbers
    )
    SELECT
        ds.TrendDate,
        COUNT(t.Id) AS TicketsCreatedCount,
        COUNT(CASE WHEN t.BreachedAt IS NOT NULL THEN 1 END) AS BreachedCount,
        CAST(
            COUNT(CASE WHEN t.BreachedAt IS NOT NULL THEN 1 END) AS FLOAT
        )
        / NULLIF(COUNT(t.Id), 0)
        * 100 AS BreachRatePercent
    FROM DateSeries ds
    LEFT JOIN dbo.Tickets t
        ON CAST(t.CreatedAt AS DATE) = ds.TrendDate
        AND t.TenantId = @TenantId
    GROUP BY ds.TrendDate
    ORDER BY ds.TrendDate ASC;
END
");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE dbo.GetSLADailyTrend;");
        }
    }
}