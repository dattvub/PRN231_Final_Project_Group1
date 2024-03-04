using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.OrderDetail
{
    public class CreateOrderDetailDto
    {
        private int _productId;

        [Required]
        public int ProductId { 
            get => _productId; 
            set => _productId = value; 
        }
    }
}
