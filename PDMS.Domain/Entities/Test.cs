using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Domain.Entities
{
    public class Test : BaseEntity<Guid>
    {
        public string? Name { get; set; }
    }
}
