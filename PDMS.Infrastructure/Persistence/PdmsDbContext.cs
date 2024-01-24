using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Abstractions;
using PDMS.Domain.Entities;

namespace PDMS.Infrastructure.Persistence
{
    public class PdmsDbContext : IdentityDbContext<
        User, Role, string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>
    >,IPdmsDbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PdmsDbContext(DbContextOptions<PdmsDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //new SeedDataApplicationDatabaseContext(builder).Seed();

            // Rename AspNet default tables
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            /*builder.Entity<UnitTreeGroup>().HasData(
                new UnitTreeGroup
                {
                    Id = Guid.Parse("379b8687-648d-4b30-a5d0-ccb3b5f21df4"),
                    UnitTreeGroup_Code = "Chưa thuộc phòng/nhóm",
                    Name = "Chưa thuộc phòng/nhóm",
                    Supervise = true,
                }
            );

            builder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = "c9f8da10-4b6c-436c-9a5c-5291f60ea8b6",
                    EmployeeName = "admin",
                    EmployeeCode = "Administrator",
                    Email = "admin@gmail.com",
                    EmployeeTitle = "Chủ sở hữu",
                    Status = true,
                    Archived = false,
                },
                new Employee
                {
                    Id = "a4a6bf8d-6734-4485-978f-e9904b7f590d",
                    EmployeeName = "supervisor",
                    EmployeeCode = "Supervisor",
                    Email = "supervisor@gmail.com",
                    EmployeeTitle = "Giám sát",
                    Status = true,
                    Archived = false,
                },
                new Employee
                {
                    Id = "01b55389-131d-4bc1-ad96-50985b2b21dd",
                    EmployeeName = "accountant",
                    EmployeeCode = "Accountant",
                    Email = "accountant@gmail.com",
                    EmployeeTitle = "Kế toán",
                    Status = true,
                    Archived = false,
                },
                new Employee
                {
                    Id = "a208843f-58f3-4b3c-89a9-90969001513c",
                    EmployeeName = "sale",
                    EmployeeCode = "Sale",
                    Email = "sale@gmail.com",
                    EmployeeTitle = "Nhân viên",
                    Status = true,
                    Archived = false,
                }
            );*/

            builder.Entity<UserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }
    }
}
