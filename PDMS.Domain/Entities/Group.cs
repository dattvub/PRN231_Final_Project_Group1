namespace PDMS.Domain.Entities;

public class Group {
    public int GroupId { get; set; }
    public string GroupCode { get; set; }
    public string GroupName { get; set; }
    public bool Status { get; set; }
    public string Address { get; set; }
    public List<Employee> Employees { get; set; }
}