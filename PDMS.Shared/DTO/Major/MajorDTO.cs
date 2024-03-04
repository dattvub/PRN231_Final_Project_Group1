using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDMS.Shared.DTO.Major
{
    public class MajorDTO
    {
        public int MajorId { get; set; }
        public string MajorCode { get; set; }
        public string MajorName { get; set; }
        public bool Status { get; set; }
        //public ProductDto? Product { get; set; }
    }
    //public class ProductDto
    //{
    //    public int ProductId { get; set; }
    //    public string ProductName { get; set; }
    //    public string? ProductCode { get; set; }
    //}
    public class MajorRequest
    {
        public int MajorId { get; set; }
        public string MajorCode { get; set; }
        public string MajorName { get; set; }
        public bool Status { get; set; }
    }
    public class GetMajorsDto
    {
        public int Page { get; set; } = 1;
        public int Quantity { get; set; } = 10;
        public string? Query { get; set; }
        public bool QueryByName { get; set; } = true;
    }
    public class CreateMajorDTO
    {
        private string _majorCode;
        private string _majorName;

        [Required]
        [MaxLength(50)]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "BrandCode must be alphanumeric")]
        public string MajorCode
        {
            get => _majorCode;
            set => _majorCode = value.Trim().ToUpper();
        }

        [Required]
        [MaxLength(50)]
        public string MajorName
        {
            get => _majorName;
            set => _majorName = value.Trim();
        }
    }
}