using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.CustomerType
{
    public class CustomerTypeDto
    {
        public int CustomerTypeId { get; set; }
        public string CustomerTypeCode { get; set; } = default!;
        public string CustomerTypeName { get; set; } = default!;
        public bool Status { get; set; }
    }
}
