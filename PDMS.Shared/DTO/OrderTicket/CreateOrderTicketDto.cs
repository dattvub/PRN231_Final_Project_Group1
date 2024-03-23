using System.ComponentModel.DataAnnotations;

namespace PDMS.Shared.DTO.OrderTicket;

public class CreateOrderTicketDto {
    public DateTime? ExpectedOrderDate { get; set; }
    public DateTime? ExpectedReceiveDate { get; set; }

    [MaxLength(400)]
    public string? Note { get; set; }

    [Required]
    [MaxLength(200)]
    public string Address { get; set; }

    [Required]
    public CartItem[] CartItems { get; set; }
}

public class CartItem {
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}