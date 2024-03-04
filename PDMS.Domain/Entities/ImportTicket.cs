namespace PDMS.Domain.Entities;

public class ImportTicket {
    public int ImportId { get; set; }
    public int TicketCode { get; set; }
    public int EmployeeId { get; set; }
    public DateTime ImportDate { get; set; }
    public int CreatorId { get; set; }
    public bool Status { get; set; }
    public double TotalPay { get; set; }
    public Employee Employee { get; set; }
    public Employee Creator { get; set; }
    public List<ImportDetail> ImportDetails { get; set; }
}