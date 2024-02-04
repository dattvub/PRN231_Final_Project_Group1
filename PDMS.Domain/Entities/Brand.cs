namespace PDMS.Domain.Entities; 

public class Brand {
    public int BrandId { get; set; }
    public string BrandCode { get; set; }
    public string BrandName { get; set; }
    public bool Status { get; set; }
    public List<Product> Products { get; set; }
}