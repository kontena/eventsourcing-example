using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kontena.EventSourcing;
using Kontena.Dashboard.Models;
using Kontena.Dashboard.Services;

namespace Kontena.Dashboard.Controllers
{
    public class ProductsController : Controller
    {
        private readonly Repository _repository;

        public ProductsController(
            Repository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("api/products")]
        public IActionResult GetProducts()
        {
            return Ok(_repository.GetProductsWithPurchases());
        }

        [HttpGet]
        [Route("api/products/{id}")]
        public IActionResult GetProduct(string id)
        {
            var products = _repository.GetProductsWithPurchases(id);

            if (!products.Any())
            {
                return NotFound(new { Id = id, Message = $"Failed to locate product with id {id}." });
            }

            return Ok(products.First());
        }

        [HttpGet]
        [Route("api/products/{id}/purchases")]
        public IActionResult GetProductPurchases(string id)
        {
            var customerPurchases = _repository.GetCustomerPurchasesForProduct(id);

            if (!customerPurchases.Any())
            {
                return NotFound(new { Id = id, Message = $"Failed to locate product purchases with id {id}." });
            }

            return Ok(customerPurchases);
        }
    }
}
