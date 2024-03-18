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

namespace PDMS.Infrastructure.Persistence {
    public class PdmsDbContext : IdentityDbContext<
            User, Role, string,
            IdentityUserClaim<string>,
            UserRole,
            IdentityUserLogin<string>,
            IdentityRoleClaim<string>,
            IdentityUserToken<string>
        >,
        IPdmsDbContext {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DbSet<CustomerType> CustomerTypes { get; set; }
        public DbSet<CustomerGroup> CustomerGroups { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OrderTicket> OrderTickets { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Major> Majors { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<ImportTicket> ImportTickets { get; set; }
        public DbSet<ImportDetail> ImportDetails { get; set; }
        public DbSet<Group> Groups { get; set; }

        public PdmsDbContext(
            DbContextOptions<PdmsDbContext> options,
            IHttpContextAccessor httpContextAccessor
        ) : base(options) {
            _httpContextAccessor = httpContextAccessor;
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            IdentityConfig(builder);
            CustomerTypeConfig(builder);
            CustomerGroupConfig(builder);
            CustomerConfig(builder);
            OrderTicketConfig(builder);
            SupplierConfig(builder);
            MajorsConfig(builder);
            BrandConfig(builder);
            EmployeeConfig(builder);
            ProductConfig(builder);
            OrderDetail(builder);
            NotificationConfig(builder);
            ImportTicketConfog(builder);
            ImportDetailConfig(builder);
            GroupConfig(builder);
        }

        private void GroupConfig(ModelBuilder builder) {
            var group = builder.Entity<Group>();
            group.HasKey(x => x.GroupId);
            group.Property(x => x.GroupId)
                .UseIdentityColumn();
            group.Property(x => x.GroupCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            group.Property(x => x.GroupName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            group.Property(x => x.Status)
                .IsRequired();
            group.Property(x => x.Address)
                .HasColumnType("nvarchar")
                .HasMaxLength(100)
                .IsRequired();
            group
                .HasIndex(x => x.GroupCode)
                .IsUnique();
        }

        private void ImportDetailConfig(ModelBuilder builder) {
            var importDetail = builder.Entity<ImportDetail>();
            importDetail.HasKey(x => x.ImportDetailId);
            importDetail.Property(x => x.ImportDetailId)
                .UseIdentityColumn();
            importDetail.Property(x => x.ImportId)
                .IsRequired();
            importDetail.Property(x => x.ProductId)
                .IsRequired();
            importDetail.Property(x => x.Quantity)
                .IsRequired();
            importDetail.Property(x => x.Price)
                .IsRequired();
            importDetail.Property(x => x.Total)
                .IsRequired();
            importDetail
                .HasOne(x => x.ImportTicket)
                .WithMany(x => x.ImportDetails)
                .HasForeignKey(x => x.ImportId)
                .OnDelete(DeleteBehavior.Restrict);
            importDetail
                .HasOne(x => x.Product)
                .WithMany(x => x.ImportDetails)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ImportTicketConfog(ModelBuilder builder) {
            var importTicket = builder.Entity<ImportTicket>();
            importTicket.HasKey(x => x.ImportId);
            importTicket.Property(x => x.ImportId)
                .UseIdentityColumn();
            importTicket.Property(x => x.TicketCode)
                .IsRequired();
            importTicket.Property(x => x.EmployeeId)
                .IsRequired();
            importTicket.Property(x => x.ImportDate)
                .HasColumnType("datetime")
                .IsRequired();
            importTicket.Property(x => x.CreatorId)
                .IsRequired();
            importTicket.Property(x => x.Status)
                .IsRequired();
            importTicket.Property(x => x.TotalPay)
                .IsRequired();
            importTicket
                .HasOne(x => x.Employee)
                .WithMany(x => x.ImportTickets)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            importTicket
                .HasOne(x => x.Creator)
                .WithMany(x => x.CreatedImportTickets)
                .HasForeignKey(x => x.CreatorId)
                .OnDelete(DeleteBehavior.Restrict);
            importTicket
                .HasIndex(x => x.TicketCode)
                .IsUnique();
        }

        private void NotificationConfig(ModelBuilder builder) {
            var notification = builder.Entity<Notification>();
            notification.HasKey(x => x.NotiId);
            notification.Property(x => x.NotiId)
                .UseIdentityColumn();
            notification.Property(x => x.Title)
                .HasColumnType("nvarchar")
                .HasMaxLength(100)
                .IsRequired();
            notification.Property(x => x.Content)
                .HasColumnType("nvarchar")
                .HasMaxLength(200)
                .IsRequired();
            notification.Property(x => x.Time)
                .HasColumnType("datetime")
                .IsRequired();
            notification.Property(x => x.Status)
                .IsRequired();
            notification
                .HasOne(x => x.OrderTicket)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            notification
                .HasOne(x => x.Employee)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
            notification
                .HasOne(x => x.CustomerCreate)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.CustomerCreateId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void OrderDetail(ModelBuilder builder) {
            var orderDetail = builder.Entity<OrderDetail>();
            orderDetail.HasKey(x => x.OrderDetailId);
            orderDetail.Property(x => x.OrderDetailId)
                .UseIdentityColumn();
            orderDetail.Property(x => x.OrderId)
                .IsRequired();
            orderDetail.Property(x => x.ProductId)
                .IsRequired();
            orderDetail.Property(x => x.Quantity)
                .IsRequired();
            orderDetail.Property(x => x.Price)
                .IsRequired();
            orderDetail.Property(x => x.Total)
                .IsRequired();
            orderDetail
                .HasOne(x => x.OrderTicket)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            orderDetail
                .HasOne(x => x.Product)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        private void ProductConfig(ModelBuilder builder) {
            var product = builder.Entity<Product>();
            product.HasKey(x => x.ProductId);
            product.Property(x => x.ProductId)
                .UseIdentityColumn();
            product.Property(x => x.ProductCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            product.Property(x => x.ProductName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            product.Property(x => x.ImportPrice)
                .IsRequired();
            product.Property(x => x.Price)
                .IsRequired();
            product.Property(x => x.Quantity)
                .IsRequired();
            product.Property(x => x.BarCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            product.Property(x => x.CreatedById)
                .IsRequired();
            product.Property(x => x.CreatedTime)
                .HasColumnType("datetime")
                .IsRequired();
            product.Property(x => x.LastModifiedById)
                .IsRequired();
            product.Property(x => x.LastModifiedTime)
                .HasColumnType("datetime")
                .IsRequired();
            product.Property(x => x.Image)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            product.Property(x => x.BrandId)
                .IsRequired();
            product.Property(x => x.SuppilerId)
                .IsRequired();
            product.Property(x => x.MajorId)
                .IsRequired();
            product.Property(x => x.Status)
                .IsRequired();
            product
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedProducts)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            product
                .HasOne(x => x.Brand)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.BrandId)
                .OnDelete(DeleteBehavior.Restrict);
            product
                .HasOne(x => x.Supplier)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.SuppilerId)
                .OnDelete(DeleteBehavior.Restrict);
            product
                .HasOne(x => x.Major)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.MajorId)
                .OnDelete(DeleteBehavior.Restrict);
            product
                .HasIndex(x => x.ProductCode)
                .IsUnique();
            product
                .HasIndex(x => x.BarCode)
                .IsUnique();
        }

        private void EmployeeConfig(ModelBuilder builder) {
            var employee = builder.Entity<Employee>();
            employee.HasKey(x => x.EmpId);
            employee.Property(x => x.EmpId)
                .UseIdentityColumn();
            employee.Property(x => x.UserId)
                .HasColumnType("nvarchar")
                .HasMaxLength(450)
                .IsRequired();
            employee.Property(x => x.EmpName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            employee.Property(x => x.EmpCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            employee.Property(x => x.Status)
                .IsRequired();
            employee.Property(x => x.Position)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);
            employee.Property(x => x.Department)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);
            employee.Property(x => x.EntranceDate)
                .HasColumnType("datetime");
            employee.Property(x => x.ExitDate)
                .HasColumnType("datetime");
            employee.Property(x => x.Address)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);
            employee.Property(x => x.Gender)
                .IsRequired();
            employee.Property(x => x.Avatar)
                .HasColumnType("nvarchar")
                .HasMaxLength(50);
            employee.Property(x => x.CreateDate)
                .HasColumnType("datetime")
                .IsRequired();
            employee
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedEmployees)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
            employee
                .HasIndex(x => x.EmpCode)
                .IsUnique();
            employee
                .HasOne(x => x.User)
                .WithOne(x => x.Employee)
                .HasForeignKey<Employee>(x => x.UserId)
                .IsRequired();
            employee
                .HasOne(x => x.Group)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.GroupId);
            employee
                .HasIndex(x => x.UserId)
                .IsUnique();
        }

        private void BrandConfig(ModelBuilder builder) {
            var brand = builder.Entity<Brand>();
            brand.HasKey(x => x.BrandId);
            brand.Property(x => x.BrandId)
                .UseIdentityColumn();
            brand.Property(x => x.BrandCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            brand.Property(x => x.BrandName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            brand.Property(x => x.Status)
                .IsRequired();
            brand
                .HasIndex(x => x.BrandCode)
                .IsUnique();
        }

        private void MajorsConfig(ModelBuilder builder) {
            var major = builder.Entity<Major>();
            major.HasKey(x => x.MajorId);
            major.Property(x => x.MajorId)
                .UseIdentityColumn();
            major.Property(x => x.MajorCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            major.Property(x => x.MajorName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            major.Property(x => x.Status)
                .IsRequired();
            major
                .HasIndex(x => x.MajorCode)
                .IsUnique();
        }

        private void SupplierConfig(ModelBuilder builder) {
            var supplier = builder.Entity<Supplier>();
            supplier.HasKey(x => x.SupplierId);
            supplier.Property(x => x.SupplierId)
                .UseIdentityColumn();
            supplier.Property(x => x.SupplierCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            supplier.Property(x => x.SupplierName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            supplier.Property(x => x.Status)
                .IsRequired();
            supplier
                .HasIndex(x => x.SupplierCode)
                .IsUnique();
        }

        private void OrderTicketConfig(ModelBuilder builder) {
            var orderTicket = builder.Entity<OrderTicket>();
            orderTicket.HasKey(x => x.OrderId);
            orderTicket.Property(x => x.OrderId)
                .UseIdentityColumn();
            orderTicket.Property(x => x.OrderCode)
                .IsRequired();
            orderTicket.Property(x => x.CustomerId)
                .IsRequired();
            orderTicket.Property(x => x.OrderDate)
                .HasColumnType("datetime")
                .IsRequired();
            orderTicket.Property(x => x.CreatedDate)
                .HasColumnType("datetime")
                .IsRequired();
            orderTicket.Property(x => x.Status)
                .IsRequired();
            orderTicket.Property(x => x.TotalPay)
                .IsRequired();
            orderTicket.Property(x => x.Discount)
                .IsRequired();
            orderTicket
                .HasOne(x => x.Customer)
                .WithMany(x => x.OrderTickets)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
            orderTicket
                .HasIndex(x => x.OrderCode)
                .IsUnique();
        }

        private void CustomerTypeConfig(ModelBuilder builder) {
            var customerType = builder.Entity<CustomerType>();
            customerType.HasKey(x => x.CustomerTypeId);
            customerType.Property(x => x.CustomerTypeId)
                .UseIdentityColumn();
            customerType.Property(x => x.CustomerTypeCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customerType.Property(x => x.CustomerTypeName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customerType.Property(x => x.Status)
                .IsRequired();
            customerType
                .HasIndex(x => x.CustomerTypeCode)
                .IsUnique();
        }

        private void CustomerGroupConfig(ModelBuilder builder) {
            var customerGroup = builder.Entity<CustomerGroup>();
            customerGroup.HasKey(x => x.CustomerGroupId);
            customerGroup.Property(x => x.CustomerGroupId)
                .UseIdentityColumn();
            customerGroup.Property(x => x.CustomerGroupCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customerGroup.Property(x => x.CustomerGroupName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customerGroup.Property(x => x.Status)
                .IsRequired();
            customerGroup
                .HasIndex(x => x.CustomerGroupCode)
                .IsUnique();
        }

        private void CustomerConfig(ModelBuilder builder) {
            var customer = builder.Entity<Customer>();
            customer.HasKey(x => x.CustomerId);
            customer.Property(x => x.CustomerId)
                .UseIdentityColumn();
            customer.Property(x => x.UserId)
                .HasColumnType("nvarchar")
                .HasMaxLength(450)
                .IsRequired();
            customer.Property(x => x.CustomerCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customer.Property(x => x.CustomerName)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customer.Property(x => x.Address)
                .HasColumnType("nvarchar")
                .HasMaxLength(50)
                .IsRequired();
            customer.Property(x => x.TaxCode)
                .HasColumnType("nvarchar")
                .HasMaxLength(13)
                .IsRequired();
            customer.Property(x => x.CustomerTypeId)
                .IsRequired();
            customer.Property(x => x.CustomerGroupId)
                .IsRequired();
            customer
                .HasOne(x => x.CustomerType)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.CustomerTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            customer
                .HasOne(x => x.CustomerGroup)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.CustomerGroupId)
                .OnDelete(DeleteBehavior.Restrict);
            customer
                .HasIndex(x => x.CustomerCode)
                .IsUnique();
            customer
                .HasIndex(x => x.TaxCode)
                .IsUnique();
            customer
                .HasOne(x => x.User)
                .WithOne(x => x.Customer)
                .HasForeignKey<Customer>(x => x.UserId)
                .IsRequired();
            customer
                .HasOne(x => x.Employee)
                .WithMany(x => x.Customers)
                .HasForeignKey(x => x.EmpId);
            customer
                .HasIndex(x => x.UserId)
                .IsUnique();
        }

        private void IdentityConfig(ModelBuilder builder) {
            builder.Entity<User>().ToTable("Users");
            builder.Entity<Role>().ToTable("Roles");
            builder.Entity<UserRole>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.Entity<UserRole>(
                userRole => {
                    userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                    userRole.HasOne(ur => ur.Role)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.RoleId)
                        .IsRequired();

                    userRole.HasOne(ur => ur.User)
                        .WithMany(r => r.UserRoles)
                        .HasForeignKey(ur => ur.UserId)
                        .IsRequired();
                }
            );
        }
    }
}