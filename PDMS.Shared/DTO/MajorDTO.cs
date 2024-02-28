using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO
{
    public class MajorDTO
    {
        public int MajorId { get; set; }
        public string MajorCode { get; set; }
        public string MajorName { get; set; }
        public bool Status { get; set; }
        public ProductDto? Product { get; set; }
    }
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductCode { get; set; }
    }
    public class MajorRequest
    {
        public int MajorId { get; set; }
        public string MajorCode { get; set; }
        public string MajorName { get; set; }
        public bool Status { get; set; }
    }
}
