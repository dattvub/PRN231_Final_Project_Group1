using PDMS.Shared.DTO.User;

namespace PDMS.Shared.DTO.Employee;

public class EmployeeDto : UserDto {
    public int EmpId { get; set; }
    public string EmpCode { get; set; } = null!;
    public string EmpName { get; set; } = null!;
    public bool Status { get; set; }
    public string? Position { get; set; }
    public string? Department { get; set; }
    public DateTime? EntranceDate { get; set; }
    public DateTime? ExitDate { get; set; }
    public string? Address { get; set; }
    public bool Gender { get; set; }
    public int? CreatedById { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public DateTime CreateDate { get; set; }
}