namespace PDMS.Shared.DTO.OrderTicket;

public class GetOrderTicketDto {
    public int Page { get; set; } = 1;
    public int Quantity { get; set; } = 10;
    public string? Query { get; set; }
}