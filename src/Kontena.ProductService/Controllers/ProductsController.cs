using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kontena.EventSourcing;
using Kontena.ProductService.Models;

namespace Kontena.ProductService.Controllers
{
    [Route("[controller]")]
    public class ProductsController : Controller
    {
        private readonly IRepository<Product> _productRepository;
        private readonly EventPublisher<Product, EventBusConfig<Product>> _eventPublisher;

        public ProductsController(
            IRepository<Product> productRepository,
            EventPublisher<Product, EventBusConfig<Product>> eventPublisher)
        {
            _productRepository = productRepository;
            _eventPublisher = eventPublisher;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_productRepository.All());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var item = _productRepository.Get(id);

            if (item == null)
                return ProductNotFound(id);

            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Product value)
        {
            if (ModelState.IsValid)
            {
                var product = value.WithNewId();
                _eventPublisher.Publish(product, EventType.Set);
                return Created($"{Request.Path.Value.TrimEnd('/')}/{product.Id}", product);
            }

            return BadRequest(ModelState);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody]Product value)
        {
            if (ModelState.IsValid)
            {
                var product = _productRepository.Get(id);

                if (product == null)
                    return ProductNotFound(id);

                product = product.WithName(value.Name)
                                 .WithPrice(value.Price);

                _eventPublisher.Publish(product, EventType.Set);

                return Ok(product);
            }

            return BadRequest(ModelState);
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var product = _productRepository.Get(id);

            if (product == null)
                return ProductNotFound(id);

            _eventPublisher.Publish(product, EventType.Remove);

            return Ok(product);
        }

        private IActionResult ProductNotFound(string id)
        {
            return NotFound(new { Id = id, Message = $"Failed to locate product with id {id}." });
        }
    }
}
