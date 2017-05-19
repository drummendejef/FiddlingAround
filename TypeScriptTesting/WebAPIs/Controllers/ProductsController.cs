using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPIs.Model;

namespace WebAPIs.Controllers
{
    //[Produces("application/json")]
    [Produces("application/xml")]
    public class ProductsController : Controller
    {
        List<Product> _products = new List<Product>
            {
                new Product { Id = 1, Name = "Awesome product", Price = 9.99f},
                new Product { Id = 2, Name = "Standard product", Price = 20},
                new Product { Id = 3, Name = "Unavailable product"}
            };

        //        [HttpGet]
        //        public string Get() => "Hello products";

        [Route("api/Products")]
        [HttpGet]
        public List<Product> Get()
        {
            return _products;
        }

        [Route("api/Products/{id}")]
        [HttpGet]
        public IActionResult Get(int id)
        {
            var product = _products
                .FirstOrDefault(p => p.Id == id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [Route("api/Products")]
        [HttpPost]
        public IActionResult Post([FromBody]Product product)
        {
            return CreatedAtAction(nameof(Get), new {id = product.Id}, product);
        }
    }
}