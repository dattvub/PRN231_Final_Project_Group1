using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Entities;

namespace PDMS.Domain.Abstractions {
    public interface IPdmsDbContext {
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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}