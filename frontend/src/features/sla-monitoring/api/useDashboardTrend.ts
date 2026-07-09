import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "@/lib/api-client";

export interface DailyTrendPoint {
  trendDate: string;
  ticketsCreatedCount: number;
  breachedCount: number;
  breachRatePercent: number | null;
}

export function useDashboardTrend(tenantId: string, daysBack: number = 14) {
  return useQuery({
    queryKey: ["dashboard-trend", tenantId, daysBack],
    queryFn: () =>
      apiFetch<DailyTrendPoint[]>(`/dashboard/trend?daysBack=${daysBack}`, {
        method: "GET",
        tenantId,
      }),
    enabled: !!tenantId,
  });
}