using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Group;

public class UpdateGroupDto {
    [MaxLength(50)]
    public string? Name { get; set; }

    [MaxLength(10)]
    public string? Address { get; set; }
}