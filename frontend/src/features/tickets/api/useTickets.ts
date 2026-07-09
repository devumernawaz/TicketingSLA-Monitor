import { useQuery } from "@tanstack/react-query";
import { apiFetch } from "@/lib/api-client";

export interface Ticket {
  id: string;
  title: string;
  description: string;
  status: string;
  priority: string;
  createdAt: string;
  slaDeadline: string;
  isBreached: boolean;
  assignedAgentId: string | null;
}

export function useTickets(tenantId: string) {
  return useQuery({
    queryKey: ["tickets", tenantId],
    queryFn: () => apiFetch<Ticket[]>("/tickets", { method: "GET", tenantId }),
  });
}