namespace PDMS.Domain.Entities;

public class Notification {
    public int NotiId { get; set; }
    public int? OrderId { get; set; }
    public int? EmployeeId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public int? CustomerCreateId { get; set; }
    public DateTime Time { get; set; }
    public bool Status { get; set; }
    public OrderTicket? OrderTicket { get; set; }
    public Employee? Employee { get; set; }
    public Customer? CustomerCreate { get; set; }
}