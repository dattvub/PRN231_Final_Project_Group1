namespace PDMS.Shared.DTO.Customer;

public class GetCustomersDto
{
    public int Page { get; set; } = 1;
    public int Quantity { get; set; } = 10;
    public string? Query { get; set; }
    public bool QueryByName { get; set; } = true;
}