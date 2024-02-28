using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Customer;

public class CreateCustomerDto
{
    private string _customerCode;
    private string _customerName;

    [Required]
    [MaxLength(50)]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "CustomerCode must be alphanumeric")]
    public string CustomerCode
    {
        get => _customerCode;
        set => _customerCode = value.Trim().ToUpper();
    }

    [Required]
    [MaxLength(50)]
    public string CustomerName
    {
        get => _customerName;
        set => _customerName = value.Trim();
    }
}