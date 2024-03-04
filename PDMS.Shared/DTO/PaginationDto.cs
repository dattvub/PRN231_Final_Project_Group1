namespace PDMS.Shared.DTO;

public class PaginationDto<T> {
    public IEnumerable<T> Items { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int ItemsPerPage { get; set; }
    public string? Query { get; set; }

    public PaginationDto(IEnumerable<T> items) {
        Items = items;
        Total = Items.Count();
        Page = 1;
        ItemsPerPage = Total;
    }

    public PaginationDto(IEnumerable<T> items, int total) {
        Items = items;
        Total = total;
        Page = 1;
        ItemsPerPage = Total;
    }
}