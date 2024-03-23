using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.Employee;

public class CreateEmployeeDto {
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
    public bool Gender { get; set; } = true;

    [Required]
    [MaxLength(256)]
    public string Email { get; set; }

    [Required]
    [MaxLength(50)]
    public string Password { get; set; }

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    public string? Role { get; set; }

    [MaxLength(50)]
    public string? Position { get; set; }

    [MaxLength(50)]
    public string? Department { get; set; }

    [MaxLength(10)]
    public string? EntranceDate { get; set; }

    [MaxLength(10)]
    public string? ExitDate { get; set; }

    public int? GroupId { get; set; }
}