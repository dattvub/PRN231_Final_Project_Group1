namespace PDMS.Domain.Entities;

public class Product {
    public int ProductId { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public double ImportPrice { get; set; }
    public double Price { get; set; }
    public int Quantity { get; set; }
    public string BarCode { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedTime { get; set; }
    public int LastModifiedById { get; set; }
    public DateTime LastModifiedTime { get; set; }
    public string Image { get; set; }
    public string Description { get; set; }
    public int BrandId { get; set; }
    public int SuppilerId { get; set; }
    public int MajorId { get; set; }
    public bool Status { get; set; }
    public Employee CreatedBy { get; set; }
    public Employee LastModifiedBy { get; set; }
    public Brand Brand { get; set; }
    public Supplier Supplier { get; set; }
    public Major Major { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
    public List<ImportDetail> ImportDetails { get; set; }
}