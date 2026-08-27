import { useMutation, useQueryClient } from "@tanstack/react-query";
import { apiFetch } from "@/lib/api-client";
import { Ticket } from "./useTickets";

export function useCloseTicket(tenantId: string) {
  const queryClient = useQueryClient();
  const queryKey = ["tickets", tenantId];

  return useMutation({
    mutationFn: (ticketId: string) =>
      apiFetch<Ticket>(`/tickets/${ticketId}/close`, { method: "PATCH", tenantId }),

    onMutate: async (ticketId: string) => {
      // 1. Stop any in-flight refetches so they don't clobber our optimistic update
      await queryClient.cancelQueries({ queryKey });

      // 2. Snapshot the current cache, so we can roll back if this fails
      const previousTickets = queryClient.getQueryData<Ticket[]>(queryKey);

      // 3. Optimistically update the cache right now, before the server responds
      queryClient.setQueryData<Ticket[]>(queryKey, (old) =>
        old?.map((ticket) =>
          ticket.id === ticketId ? { ...ticket, status: "Closed" } : ticket
        )
      );

      // 4. Return the snapshot — TanStack Query passes this to onError as `context`
      return { previousTickets };
    },

    onError: (_err, _ticketId, context) => {
      // Roll back to the snapshot taken in onMutate
      if (context?.previousTickets) {
        queryClient.setQueryData(queryKey, context.previousTickets);
      }
    },

    onSettled: () => {
      // Whether it succeeded or failed, refetch to reconcile with real server state
      queryClient.invalidateQueries({ queryKey });
    },
  });
}