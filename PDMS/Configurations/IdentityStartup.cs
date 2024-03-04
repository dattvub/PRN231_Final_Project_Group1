using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;
using PDMS.Shared.Constants;

namespace PDMS.Configurations
{
    public static class IdentityConfiguration
    {
        public static IApplicationBuilder UseApplicationIdentity(this IApplicationBuilder builder,
            IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

                SeedRoles(roleManager).Wait();
                SeedUsers(userManager, serviceProvider.GetRequiredService<IPdmsDbContext>()).Wait();
                SeedUserRoles(userManager).Wait();
            }

            return builder;
        }


        private static IEnumerable<Role> Roles()
        {
            return new List<Role> {
                new Role {Id = "director", Name = RolesConstants.DIRECTOR},
                new Role {Id = "customer",Name = RolesConstants.CUSTOMER},
                new Role {Id = "salemans",Name = RolesConstants.SALEMAN},
                new Role {Id = "supervisor",Name = RolesConstants.SUPERVISOR},
                new Role {Id = "accountant",Name = RolesConstants.ACCOUNTANT},
            };
        }

        private static IEnumerable<User> Users()
        {
            return new List<User> {
                new User {
                    Id = "c9f8da10-4b6c-436c-9a5c-5291f60ea8b6",
                    UserName = "director",
                    PasswordHash = "$2y$10$Qv4U/vn/Li6RHhJtW.2yNug4L5BExm/B7eamFs7z36l/KkryfyUKa",
                    FirstName = "director",
                    LastName = "",
                    Email = "director@gmail.com",
                    Activated = true,
                    LangKey = "en"
                },
            };
        }

        private static IDictionary<string, string[]> UserRoles()
        {
            return new Dictionary<string, string[]> {
                {"c9f8da10-4b6c-436c-9a5c-5291f60ea8b6", new[] { "DIRECTOR"}},
            };
        }

        private static async Task SeedRoles(RoleManager<Role> roleManager)
        {
            foreach (var role in Roles())
            {
                var dbRole = await roleManager.FindByNameAsync(role.Name);
                if (dbRole == null)
                {
                    await roleManager.CreateAsync(role);
                }
                else
                {
                    await roleManager.UpdateAsync(dbRole);
                }
            }
        }

        private static async Task SeedUsers(UserManager<User> userManager, IPdmsDbContext dbContext)
        {
            foreach (var user in Users())
            {
                var dbUser = await userManager.FindByIdAsync(user.Id);
                if (dbUser == null)
                {
                    await userManager.CreateAsync(user);
                }
                else
                {
                    await userManager.UpdateAsync(dbUser);
                }

                if (await dbContext.Employees.FirstOrDefaultAsync(x => x.UserId == user.Id) != null) {
                    continue;
                }
                await dbContext.Employees.AddAsync(new Employee() {
                    EmpName = $"{user.LastName} {user.FirstName}".Trim(),
                    EmpCode = user.UserName,
                    UserId = user.Id,
                    Status = true,
                    Gender = true,
                    CreateDate = new DateTime(2000, 1, 1)
                });
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedUserRoles(UserManager<User> userManager)
        {
            foreach (var (id, roles) in UserRoles())
            {
                var user = await userManager.FindByIdAsync(id);
                await userManager.AddToRolesAsync(user, roles);
            }
        }
    }
}
