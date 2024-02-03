namespace PDMS.Web.Models.Sidebar;

public class SidebarSection : List<SidebarItem> {
    public string? Title { get; set; }

    public SidebarSection(string? title = null) {
        Title = title;
    }
}