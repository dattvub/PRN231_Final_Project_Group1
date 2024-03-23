using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Statistic
{
    public class StatisticDto
    {
        public int CustomerCount { get; set; }
        public int EmployeeCount { get; set; }
        public int OrderPendingCount { get; set; }
        public int OrderCompleteCount { get; set; }
        public int ProductCount { get; set; }
    }
}
