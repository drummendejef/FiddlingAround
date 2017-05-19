using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RoutingMVC.Models;

namespace RoutingMVC.Controllers
{
    public class VentigrateController : Controller
    {
        [HttpGet("/")]
        public string Index() => "Hello Ventigrate!";

        [HttpGet("/vntgView")]
        public IActionResult IndexView()
        {
            return View();
        }

        [HttpGet("/vntg")]
        public IActionResult IndexVntg()
        {
            return View("VntgView");
        }

        [HttpGet("/vntgModel")]
        public IActionResult VentigrateModel()
        {
            var model = new VentigrateModel
            {
                Id = 1,
                Name = "Mr Ventigrate",
                Title = "Senior Employee"
            };

            return View(model);
        }
    }
}