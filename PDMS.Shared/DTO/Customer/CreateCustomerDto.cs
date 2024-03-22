using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Customer;

public class CreateCustomerDto
{
    [Required]
    [MaxLength(30)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; }

    [Required]
    [MaxLength(50)]
    public string Code { get; set; }

    [Required]
    [MaxLength(256)]
    public string Email { get; set; }

    [Required]
    [MaxLength(50)]
    public string Password { get; set; }

    [MaxLength(20)]
    public string Phone { get; set; }

    [Required]
    [MaxLength(13)]
    public string TaxCode { get; set; }

    [MaxLength(50)]
    public string Address { get; set; }

    public string? Role { get; set; }

    [Required]
    public int CustomerTypeId { get; set; }

    [Required]
    public int CustomerGroupId { get; set; }
    [Required]
    public int EmpId { get; set; }
}