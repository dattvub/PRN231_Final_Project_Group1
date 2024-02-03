namespace PDMS.Web.Models.Sidebar;

public class SidebarItem : List<SidebarItem> {
    public string? Icon { get; set; }
    public string Name { get; set; }
    public string Href { get; set; }

    public SidebarItem(string name, string href = "", string? icon = null) {
        Icon = icon;
        Name = name;
        Href = href;
    }
}