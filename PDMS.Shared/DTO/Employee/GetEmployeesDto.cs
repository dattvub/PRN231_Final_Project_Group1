namespace PDMS.Shared.DTO.Employee;

public class GetEmployeesDto {
    public int Page { get; set; } = 1;
    public int Quantity { get; set; } = 10;
    public string? Query { get; set; }
    public int InGroup { get; set; } = -1;
}