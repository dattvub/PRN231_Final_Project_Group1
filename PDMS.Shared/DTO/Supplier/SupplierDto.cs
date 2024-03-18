namespace PDMS.Shared.DTO.Supplier;

public class SupplierDto {
    public int SupplierId { get; set; }
    public string SupplierCode { get; set; } = null!;
    public string SupplierName { get; set; } = null!;
    public bool Status { get; set; }
}