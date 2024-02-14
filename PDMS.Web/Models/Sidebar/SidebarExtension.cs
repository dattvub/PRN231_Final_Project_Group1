using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Models.Sidebar;

public static class SidebarExtension {
    internal static readonly string Key = new Guid().ToString("N");

    public static void AddSideBar(this Controller controller, IEnumerable<SidebarSection> sections) {
        controller.ViewData[Key] = sections;
    }
}