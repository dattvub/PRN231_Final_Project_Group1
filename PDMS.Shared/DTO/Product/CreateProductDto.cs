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
        //private int _quality;
        private string _barCode;
        private int _createdById;
        //private DateTime _createdTime;
        private int _lastModifiedById;
        //private DateTime _lastModifiedTime;
        private string _image;
        private string _description;
        private int _brandId;
        private int _suppilerId;
        private int _majorId;
        //private bool _status;

        [Required]
        [MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Product name must be alphanumeric")]
        public string ProductName
        {
            get => _productName;
            set => _productName = value.Trim().ToUpper();
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
        public string Image { get => _image; set => _image = value; }
        public string Description { get => _description; set => _description = value; }
        [Required]
        public int BrandId { get => _brandId; set => _brandId = value; }
        [Required]
        public int SuppilerId { get => _suppilerId; set => _suppilerId = value; }
        [Required]
        public int MajorId { get => _majorId; set => _majorId = value; }
    }
}
