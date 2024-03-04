namespace PDMS.Domain.Entities;

public class EmpGroup {
    public int EmpId { get; set; }
    public int GroupId { get; set; }
    public Employee Employee { get; set; }
    public Group Group { get; set; }
}