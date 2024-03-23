namespace PDMS.Domain.Entities; 

public class Major {
    public int MajorId { get; set; }
    public string MajorCode { get; set; }
    public string MajorName { get; set; }
    public bool Status { get; set; }
    public List<Product> Products { get; set; }
}