using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Models.Sidebar;

public static class SidebarExtension {
    internal static readonly string Key = Guid.NewGuid().ToString("N");

    public static void AddSideBar(this Controller controller, IEnumerable<SidebarSection> sections) {
        controller.ViewData[Key] = sections;
    }
}