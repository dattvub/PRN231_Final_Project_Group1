using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Notifications
{
    public class CreateNotificationDto
    {
        private int? _orderId;
        private int? _employeeId;
        private string _title;
        private string _content;
        private int? _customerCreateId;
        //private DateTime _time;
        //private bool _status;
        [Required]
        public int? OrderId
        {
            get => _orderId;
            set => _orderId = value;
        }
        [Required]
        public int? EmployeeId
        {
            get => _employeeId;
            set => _employeeId = value;
        }
        [Required]
        public string Title
        {
            get => _title;
            set => _title = value;
        }
        [Required]
        public string Content
        {
            get => _content;
            set => _content = value;
        }
        [Required]
        public int? CustomerCreateId
        {
            get => _customerCreateId;
            set => _customerCreateId = value;
        }
    }
}
