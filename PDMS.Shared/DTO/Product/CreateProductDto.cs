using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Product
{
    public class CreateProductDto
    {
        private string _productName;
        private double _importPrice;
        private double _price;
        private string _barCode;
        private int _createdById;
        private int _lastModifiedById;
        private string _description;
        private int _brandId;
        private int _suppilerId;
        private int _majorId;

        [Required]
        public string ProductName
        {
            get => _productName;
            set => _productName = value.Trim();
        }
        [Required]
        public double ImportPrice 
        { 
            get => _importPrice; 
            set => _importPrice = value; 
        }

        [Required]
        public double Price 
        { 
            get => _price; 
            set => _price = value; 
        }
        [Required]
        public string BarCode 
        { 
            get => _barCode; 
            set => _barCode = value; }
        [Required]
        public int CreatedById { get => _createdById; set => _createdById = value; }
        [Required]
        public int LastModifiedById { get => _lastModifiedById; set => _lastModifiedById = value; }
        [Required]
        public string Description { get => _description; set => _description = value; }
        [Required]
        public int BrandId { get => _brandId; set => _brandId = value; }
        [Required]
        public int SuppilerId { get => _suppilerId; set => _suppilerId = value; }
        [Required]
        public int MajorId { get => _majorId; set => _majorId = value; }
    }
}
