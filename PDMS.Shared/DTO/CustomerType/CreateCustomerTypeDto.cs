using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.CustomerType
{
    public class CreateCustomerTypeDto
    {
        private string _customerTypeCode;
        private string _customerTypeName;
        [Required]
        [MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "CustomerTypeCode must be alphanumeric")]
        public string CustomerTypeCode
        {
            get => _customerTypeCode;
            set => _customerTypeCode = value.Trim().ToUpper();
        }

        [Required]
        [MaxLength(50)]
        public string CustomerTypeName
        {
            get => _customerTypeName;
            set => _customerTypeName = value.Trim();
        }
    }
}
