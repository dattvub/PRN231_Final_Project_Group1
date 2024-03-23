namespace PDMS.Domain.Entities;

public class OrderDetail {
    public int OrderDetailId { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double Total { get; set; }
    public OrderTicket OrderTicket { get; set; }
    public Product Product { get; set; }
}