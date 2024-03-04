namespace PDMS.Domain.Entities;

public class ImportDetail {
    public int ImportDetailId { get; set; }
    public int ImportId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public double Total { get; set; }
    public ImportTicket ImportTicket { get; set; }
    public Product Product { get; set; }
}