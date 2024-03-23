using PDMS.Shared.DTO.OrderDetail;

namespace PDMS.Shared.DTO.OrderTicket;

public class OrderTicketDto {
    public int OrderId { get; set; }
    public string OrderCode { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; }
    public string CustomerPhone { get; set; }
    public DateTime? ExpectedOrderDate { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? ExpectedReceiveDate { get; set; }
    public DateTime? ReceiveDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public double TotalPay { get; set; }
    public string Address { get; set; }
    public int Discount { get; set; }
    public List<OrderDetailDto> OrderDetails { get; set; }
}