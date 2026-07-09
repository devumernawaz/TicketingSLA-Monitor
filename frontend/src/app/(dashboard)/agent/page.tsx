"use client";

import { useAuth } from "@/lib/auth";
import { useTickets } from "@/features/tickets/api/useTickets";
import { useCloseTicket } from "@/features/tickets/api/useCloseTicket";

export default function AgentDashboardPage() {
  const { user } = useAuth();
  const tenantId = user?.tenantId ?? "";
  const { data: tickets, isLoading, error } = useTickets(tenantId);
  const closeTicket = useCloseTicket(tenantId);

  if (isLoading) return <div className="p-8">Loading tickets...</div>;
  if (error) return <div className="p-8 text-red-600">Failed to load tickets: {(error as Error).message}</div>;

  return (
    <div className="p-8">
      <h1 className="text-xl font-semibold mb-4">Agent Dashboard — Tickets</h1>
      <div className="space-y-2">
        {tickets?.map((ticket) => (
          <div key={ticket.id} className="border rounded p-3 bg-white flex justify-between items-center">
            <div>
              <div className="font-medium">{ticket.title}</div>
              <div className="text-sm text-slate-500">
                Status: {ticket.status} · Priority: {ticket.priority}
                {ticket.isBreached && <span className="ml-2 text-red-600 font-semibold">BREACHED</span>}
              </div>
            </div>
            {ticket.status !== "Closed" && (
              <button
                onClick={() => closeTicket.mutate(ticket.id)}
                className="px-3 py-1 bg-slate-700 text-white rounded text-sm"
              >
                Close
              </button>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}