namespace PDMS.Shared.DTO.Brand;

public class BrandDto {
    public int BrandId { get; set; }
    public string BrandCode { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public bool Status { get; set; }
}