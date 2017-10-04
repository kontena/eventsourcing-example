using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kontena.EventSourcing;
using Kontena.CustomerService.Models;

namespace Kontena.CustomerService.Controllers
{
    [Route("[controller]")]
    public class CustomersController : Controller
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly EventPublisher<Customer, EventBusConfig<Customer>> _eventPublisher;

        public CustomersController(
            IRepository<Customer> customerRepository,
            EventPublisher<Customer, EventBusConfig<Customer>> eventPublisher)
        {
            _customerRepository = customerRepository;
            _eventPublisher = eventPublisher;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_customerRepository.All());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var item = _customerRepository.Get(id);

            if (item == null)
                return CustomerNotFound(id);

            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Customer value)
        {
            if (ModelState.IsValid)
            {
                var customer = value.WithNewId();
                _eventPublisher.Publish(customer, EventType.Set);
                return Created($"{Request.Path.Value.TrimEnd('/')}/{customer.Id}", customer);
            }

            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]Customer value)
        {
            if (ModelState.IsValid)
            {
                var customer = _customerRepository.Get(id);

                if (customer == null)
                    return CustomerNotFound(id);

                customer = customer.WithFirstName(value.FirstName)
                                   .WithLastName(value.LastName);

                _eventPublisher.Publish(customer, EventType.Set);

                return Ok(customer);
            }

            return BadRequest(ModelState);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var customer = _customerRepository.Get(id);

            if (customer == null)
                return CustomerNotFound(id);

            _eventPublisher.Publish(customer, EventType.Remove);

            return Ok(customer);
        }

        private IActionResult CustomerNotFound(string id)
        {
            return NotFound(new { Id = id, Message = $"Failed to locate customer with id {id}." });
        }
    }
}
