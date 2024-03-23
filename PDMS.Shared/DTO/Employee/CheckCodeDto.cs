using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Employee;

public class CheckCodeDto {
    [Required]
    public string Code { get; set; }

    public int? Id { get; set; }
}