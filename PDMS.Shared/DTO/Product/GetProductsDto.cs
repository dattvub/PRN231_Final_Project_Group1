using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Product
{
    public class GetProductsDto
    {
        public int Page { get; set; } = 1;
        public int Quantity { get; set; } = 10;
        public string? Query { get; set; }
        public bool QueryByName { get; set; } = true;
        public int[]? OnlyIn { get; set; }
    }
}
