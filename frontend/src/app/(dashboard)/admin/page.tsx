"use client";

import { useAuth } from "@/lib/auth";
import { useDashboardSummary } from "@/features/sla-monitoring/api/useDashboardSummary";
import { useDashboardTrend } from "@/features/sla-monitoring/api/useDashboardTrend";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from "recharts";

function StatCard({ label, value }: { label: string; value: string | number }) {
  return (
    <div className="bg-white rounded-lg border p-4">
      <div className="text-sm text-slate-500">{label}</div>
      <div className="text-2xl font-semibold mt-1">{value}</div>
    </div>
  );
}

export default function AdminDashboardPage() {
  const { user } = useAuth();
  const tenantId = user?.tenantId ?? "";

  const { data: summary, isLoading: summaryLoading } = useDashboardSummary(tenantId);
  const { data: trend, isLoading: trendLoading } = useDashboardTrend(tenantId);

  return (
    <div className="p-8 space-y-6">
      <h1 className="text-xl font-semibold">Admin Dashboard — SLA Overview</h1>

      {summaryLoading ? (
        <div>Loading summary...</div>
      ) : (
        <div className="grid grid-cols-4 gap-4">
          <StatCard label="Open Tickets" value={summary?.openTicketCount ?? 0} />
          <StatCard label="At Risk (< 1hr)" value={summary?.atRiskCount ?? 0} />
          <StatCard label="Breached" value={summary?.breachedCount ?? 0} />
          <StatCard
            label="Avg Response Time"
            value={summary?.avgResponseTimeMinutes != null ? `${Math.round(summary.avgResponseTimeMinutes)} min` : "—"}
          />
        </div>
      )}

      <div className="bg-white rounded-lg border p-4">
        <h2 className="text-sm font-medium text-slate-500 mb-4">Tickets Created — Last 14 Days</h2>
        {trendLoading ? (
          <div>Loading trend...</div>
        ) : (
          <ResponsiveContainer width="100%" height={250}>
            <LineChart data={trend}>
              <CartesianGrid strokeDasharray="3 3" />
              <XAxis dataKey="trendDate" tickFormatter={(d) => d.slice(5)} />
              <YAxis allowDecimals={false} />
              <Tooltip />
              <Line type="monotone" dataKey="ticketsCreatedCount" stroke="#0f172a" strokeWidth={2} name="Tickets Created" />
            </LineChart>
          </ResponsiveContainer>
        )}
      </div>
    </div>
  );
}