using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Authentication;

public class LoginDto {
    [Required]
    [MaxLength(50)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; } = false;
}