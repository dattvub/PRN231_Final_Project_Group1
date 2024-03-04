using PDMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Product
{
    public class ProductDto
    {
        //public int ProductId { get; set; }
        public string ProductCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public double ImportPrice { get; set; }
        public double Price { get; set; }
        //public int Quality { get; set; }
        public string BarCode { get; set; } = null!;
        public int CreatedById { get; set; }
        //public DateTime CreatedTime { get; set; }
        public int LastModifiedById { get; set; }
        //public DateTime LastModifiedTime { get; set; }
        public string Image { get; set; } = null!;
        public int BrandId { get; set; }
        public int SuppilerId { get; set; }
        public int MajorId { get; set; }
        //public bool Status { get; set; }
    }
}
