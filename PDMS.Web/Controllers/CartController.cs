﻿using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

public class CartController : Controller {
    public IActionResult Index() {
        return View();
    }
}