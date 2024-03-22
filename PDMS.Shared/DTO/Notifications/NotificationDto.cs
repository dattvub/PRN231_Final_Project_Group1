using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Notifications
{
    public class NotificationDto
    {
        public int NotiId { get; set; }
        public int? OrderId { get; set; }
        public int? EmployeeId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? CustomerCreateId { get; set; }
        public DateTime Time { get; set; }
        public bool Status { get; set; }

    }
}
