using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.CustomerGroup;

public class CreateCustomerGroupsDto
{
    private string _customerGroupCode;
    private string _customerGroupName;

    [Required]
    [MaxLength(50)]
    [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "CustomerGroupCode must be alphanumeric")]
    public string CustomerGroupCode
    {
        get => _customerGroupCode;
        set => _customerGroupCode = value.Trim().ToUpper();
    }

    [Required]
    [MaxLength(50)]
    public string CustomerGroupName
    {
        get => _customerGroupName;
        set => _customerGroupName = value.Trim();
    }
}