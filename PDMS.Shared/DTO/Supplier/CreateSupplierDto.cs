using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Supplier;

public class CreateSupplierDto {
    private string _supplierCode;
    private string _supplierName;

    [Required]
    [MaxLength(50)]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "SupplierCode must be alphanumeric")]
    public string SupplierCode {
        get => _supplierCode;
        set => _supplierCode = value.Trim().ToUpper();
    }

    [Required]
    [MaxLength(50)]
    public string SupplierName {
        get => _supplierName;
        set => _supplierName = value.Trim();
    }
}