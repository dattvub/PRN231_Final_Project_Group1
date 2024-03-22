using PDMS.Shared.DTO.User;

namespace PDMS.Shared.DTO.Customer;

public class CustomerDto : UserDto {
    public int CustomerId { get; set; }
    public string UserId { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string TaxCode { get; set; }
    public int CustomerTypeId { get; set; }
    public int CustomerGroupId { get; set; }
    public int? EmpId { get; set; }


}