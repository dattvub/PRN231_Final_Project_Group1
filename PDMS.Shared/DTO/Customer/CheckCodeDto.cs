using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Customer;

public class CheckCusCodeDto {
    [Required]
    public string Code { get; set; }

    public int? Id { get; set; }
}