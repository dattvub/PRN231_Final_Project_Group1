using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Group;

public class CreateGroupDto {
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [MaxLength(10)]
    public string Address { get; set; }
}