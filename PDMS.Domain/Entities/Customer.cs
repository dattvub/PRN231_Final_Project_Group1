namespace PDMS.Domain.Entities;

public class Customer {
    public int CustomerId { get; set; }
    public string UserId { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string TaxCode { get; set; }
    public int CustomerTypeId { get; set; }
    public int CustomerGroupId { get; set; }
    public int? EmpId { get; set; }
    public CustomerType CustomerType { get; set; }
    public CustomerGroup CustomerGroup { get; set; }
    public List<OrderTicket> OrderTickets { get; set; }
    public List<Notification> Notifications { get; set; }
    public User User { get; set; } = null!;
    public Employee? Employee { get; set; }
}