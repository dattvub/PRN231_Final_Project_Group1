namespace PDMS.Shared.DTO.Customer;

public class CustomerDto {
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; } = null!;
    public string CustomerName { get; set; } = null!;
    public bool Status { get; set; }
}