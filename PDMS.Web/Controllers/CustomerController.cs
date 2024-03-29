﻿using Microsoft.AspNetCore.Mvc;

namespace PDMS.Web.Controllers;

[Route("[controller]")]
public class CustomerController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [Route("Create")]
    public IActionResult Create()
    {
        return View();
    }

    [Route("{id:int:min(1)}")]
    public IActionResult CusDetail([FromRoute] int id)
    {
        ViewData["cusId"] = id;
        return View();
    }

    [Route("{id:int:min(1)}/Edit")]
    public IActionResult Edit([FromRoute] int id)
    {
        ViewData["cusId"] = id;
        return View();
    }
}

//using Microsoft.AspNetCore.Mvc;

//namespace PDMS.Web.Controllers;

//[Route("[controller]")]
//public class CustomerController : Controller
//{
//    public IActionResult Index()
//    {
//        return View();
//    }

//    [Route("Create")]
//    public IActionResult Create()
//    {
//        return View();
//    }

//    [Route("{id:int:min(1)}")]
//    public IActionResult CusDetail([FromRoute] int id)
//    {
//        return View();
//    }
//}