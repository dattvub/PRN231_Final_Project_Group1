using PDMS.Shared.Enums;

namespace PDMS.Domain.Entities;

public class OrderTicket {
    public int OrderId { get; set; }
    public int OrderCode { get; set; }
    public int CustomerId { get; set; }
    public DateTime? ExpectedOrderDate { get; set; }
    public DateTime? OrderDate { get; set; }
    public DateTime? ExpectedReceiveDate { get; set; }
    public DateTime? ReceiveDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public OrderTicketStatus Status { get; set; }
    public string? Note { get; set; }
    public string? Reason { get; set; }
    public double TotalPay { get; set; }
    public string Address { get; set; }
    public int Discount { get; set; }
    public Customer Customer { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
    public List<Notification> Notifications { get; set; }
}