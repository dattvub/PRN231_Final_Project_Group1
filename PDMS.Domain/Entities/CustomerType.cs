namespace PDMS.Domain.Entities;

public class CustomerType {
    public int CustomerTypeId { get; set; }
    public string CustomerTypeCode { get; set; }
    public string CustomerTypeName { get; set; }
    public bool Status { get; set; }
    public List<Customer> Customers { get; set; }
}