namespace PDMS.Domain.Entities;

public class Employee {
    public int EmpId { get; set; }
    public string UserId { get; set; }
    public string EmpCode { get; set; }
    public string EmpName { get; set; }
    public bool Status { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public DateTime? EntranceDate { get; set; }
    public DateTime? ExitDate { get; set; }
    public string? Address { get; set; }
    public bool Gender { get; set; }
    public string? Avatar { get; set; }
    public int? CreatedById { get; set; }
    public int? GroupId { get; set; }
    public DateTime CreateDate { get; set; }
    public Employee? CreatedBy { get; set; }
    public List<Employee> CreatedEmployees { get; set; }
    public List<Product> CreatedProducts { get; set; }
    public List<Notification> Notifications { get; set; }
    public List<ImportTicket> ImportTickets { get; set; }
    public List<ImportTicket> CreatedImportTickets { get; set; }
    public User User { get; set; } = null!;
    public List<Customer> Customers { get; set; } = null!;
    public Group? Group { get; set; }
    public List<Group> ManageGroups { get; set; }
}