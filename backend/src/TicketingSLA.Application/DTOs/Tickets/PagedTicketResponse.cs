namespace TicketingSLA.Application.DTOs.Tickets;

public class PagedTicketResponse
{
    public IEnumerable<TicketResponse> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
