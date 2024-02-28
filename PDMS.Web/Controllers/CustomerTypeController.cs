using Microsoft.AspNetCore.Mvc;
using PDMS.Web.Models.Sidebar;

namespace PDMS.Web.Controllers;

[Route("Category/CustomerType")]
public class CustomerTypeController : Controller {
    public CustomerTypeController()
    {
    }

    [HttpGet]
    public IActionResult Index() {
        return View();
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