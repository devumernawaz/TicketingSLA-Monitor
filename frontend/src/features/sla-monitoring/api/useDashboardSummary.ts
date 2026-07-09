import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "@/lib/api-client";

export interface DashboardSummary {
  openTicketCount: number;
  atRiskCount: number;
  breachedCount: number;
  avgResponseTimeMinutes: number | null;
  breachRateLast24HoursPercent: number | null;
}

export function useDashboardSummary(tenantId: string) {
  return useQuery({
    queryKey: ["dashboard-summary", tenantId],
    queryFn: () => apiFetch<DashboardSummary>("/dashboard/summary", { method: "GET", tenantId }),
    enabled: !!tenantId,
  });
}