namespace PDMS.Domain.Entities;

public class CustomerGroup {
    public int CustomerGroupId { get; set; }
    public string CustomerGroupCode { get; set; }
    public string CustomerGroupName { get; set; }
    public bool Status { get; set; }
    public List<Customer> Customers { get; set; }
}