using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Brand;

public class CreateBrandDto {
    private string _brandCode;
    private string _brandName;

    [Required]
    [MaxLength(50)]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "BrandCode must be alphanumeric")]
    public string BrandCode {
        get => _brandCode;
        set => _brandCode = value.Trim().ToUpper();
    }

    [Required]
    [MaxLength(50)]
    public string BrandName {
        get => _brandName;
        set => _brandName = value.Trim();
    }
}