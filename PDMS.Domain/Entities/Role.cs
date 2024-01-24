using Microsoft.AspNetCore.Identity;

namespace PDMS.Domain.Entities
{
    public class Role : IdentityRole<string>
    {
        public ICollection<UserRole> UserRoles { get; set; }
    }
}
