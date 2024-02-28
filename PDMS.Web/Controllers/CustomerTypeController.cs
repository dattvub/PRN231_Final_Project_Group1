using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PDMS.Domain.Entities;
using PDMS.Infrastructure.Persistence;
using PDMS.Web.Models.Sidebar;

namespace PDMS.Web.Controllers;

[Route("Category/CustomerType")]
public class CustomerTypeController : Controller {
    [BindProperty(SupportsGet = true)]
    public IList<CustomerType> CustomerTypes { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public int CustomerTypeId { get; set; } = default!;

    public CustomerTypeController()
    {
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerType>>> Index() {
        addSidebar();
        CustomerTypes = new List<CustomerType>() {
            new CustomerType{ CustomerTypeId = 1,CustomerTypeCode = "zero",CustomerTypeName="segg", Status= true},
            new CustomerType { CustomerTypeId = 2, CustomerTypeCode = "one", CustomerTypeName = "segg", Status = false },
            new CustomerType { CustomerTypeId = 3, CustomerTypeCode = "two", CustomerTypeName = "segg", Status = true },
            new CustomerType { CustomerTypeId = 4, CustomerTypeCode = "three", CustomerTypeName = "segg", Status = false },
            new CustomerType { CustomerTypeId = 5, CustomerTypeCode = "four", CustomerTypeName = "segg", Status = true },
            new CustomerType { CustomerTypeId = 6, CustomerTypeCode = "five", CustomerTypeName = "segg", Status = false },
            new CustomerType { CustomerTypeId = 7, CustomerTypeCode = "six", CustomerTypeName = "segg", Status = true },
            new CustomerType { CustomerTypeId = 8, CustomerTypeCode = "seven", CustomerTypeName = "segg", Status = false },
            new CustomerType { CustomerTypeId = 9, CustomerTypeCode = "eight", CustomerTypeName = "segg", Status = true },
            new CustomerType { CustomerTypeId = 10, CustomerTypeCode = "nine", CustomerTypeName = "segg", Status = false }};
        return View(CustomerTypes);
    }

    [Route("Add")]
    [HttpGet]
    public async Task<ActionResult<List<CustomerType>>> AddCustomerType()
    {
        addSidebar();
        return View();
    }

    [Route("{id}/Update")]
    [HttpGet]
    public async Task<ActionResult<List<CustomerType>>> UpdateCustomerType([FromRoute] int id)
    {
        addSidebar();
        CustomerTypeId = id;
        return View(CustomerTypeId);
    }

    public void addSidebar()
    {
        var sections = new List<SidebarSection>() {
            new SidebarSection() {
                new SidebarItem("Dashboard", "/", "fa-solid:home") {
                    new SidebarItem("Default", "/"),
                    new SidebarItem("Analytics", "/"),
                    new SidebarItem("CRM", "/"),
                    new SidebarItem("E commerce", "/"),
                }
            },
            new SidebarSection("Pages") {
                new SidebarItem("Starter", "/"),
                new SidebarItem("Landing", "/"),
                new SidebarItem("Authentication", "/") {
                    new SidebarItem("Simple") {
                        new SidebarItem("Login", "/"),
                        new SidebarItem("Logout", "/"),
                        new SidebarItem("Register", "/"),
                        new SidebarItem("Forgot password", "/"),
                        new SidebarItem("Confirm mail", "/"),
                        new SidebarItem("Reset password", "/"),
                        new SidebarItem("Lock screen", "/"),
                    },
                    new SidebarItem("Card", "/")
                },
                new SidebarItem("User", "/"),
            }
        };
        this.AddSideBar(sections);

    }

}