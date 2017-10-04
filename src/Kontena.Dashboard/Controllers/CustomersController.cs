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
    public class CustomersController : Controller
    {
        private readonly Repository _repository;

        public CustomersController(
            Repository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("api/customers")]
        public IActionResult GetCustomers()
        {
            return Ok(_repository.GetCustomersWithPurchases());
        }

        [HttpGet]
        [Route("api/customers/{id}")]
        public IActionResult GetCustomer(string id)
        {
            var customers = _repository.GetCustomersWithPurchases(id);

            if (!customers.Any())
            {
                return NotFound(new { Id = id, Message = $"Failed to locate customer with id {id}." });
            }

            return Ok(customers.First());
        }

        [HttpGet]
        [Route("api/customers/{id}/purchases")]
        public IActionResult GetCustomerPurchases(string id)
        {
            var customerPurchases = _repository.GetCustomerPurchasesForCustomer(id);

            if (!customerPurchases.Any())
            {
                return NotFound(new { Id = id, Message = $"Failed to locate customer purchases with id {id}." });
            }

            return Ok(customerPurchases);
        }
    }
}
