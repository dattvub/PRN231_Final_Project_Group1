﻿using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("[controller]")]
public class EmployeeController : Controller {
    public IActionResult Index() {
        return View();
    }

    [Route("Create")]
    public IActionResult Create() {
        return View();
    }

    [Route("{id:int:min(1)}")]
    public IActionResult EmpDetail([FromRoute] int id) {
        ViewData["empId"] = id;
        return View();
    }

    [Route("{id:int:min(1)}/Edit")]
    public IActionResult Edit([FromRoute] int id) {
        ViewData["empId"] = id;
        return View();
    }
}