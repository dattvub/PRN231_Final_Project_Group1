﻿namespace PDMS.Shared.DTO.User;

public class UserDto {
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Role { get; set; }
    public string ImageUrl { get; set; }
    public int? AssociationId { get; set; }
}