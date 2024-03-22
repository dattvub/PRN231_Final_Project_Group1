using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class OrderTicketController : Controller {
    public IActionResult Index() {
        return View();
    }

    [HttpPost]
    public IActionResult Create(CartItem[] cartItems) {
        ViewData["data"] = JsonSerializer.Serialize(cartItems);
        return View();
    }

    public IActionResult Detail() {
        return View();
    }

    public IActionResult Update() {
        return View();
    }
}

public class CartItem {
    [Required]
    public int ProductId { get; set; }

    [Required]
    public int Quantity { get; set; }
}