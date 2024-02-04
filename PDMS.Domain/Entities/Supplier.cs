namespace PDMS.Domain.Entities; 

public class Supplier {
    public int SupplierId { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public bool Status { get; set; }
    public List<Product> Products { get; set; }
}