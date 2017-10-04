using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Kontena.EventSourcing;
using Kontena.PurchaseService.Models;

namespace Kontena.PurchaseService.Controllers
{
    [Route("[controller]")]
    public class PurchasesController : Controller
    {
        private readonly IRepository<Purchase> _purchaseRepository;
        private readonly EventPublisher<Purchase, EventBusConfig<Purchase>> _eventPublisher;

        public PurchasesController(
            IRepository<Purchase> purchaseRepository,
            EventPublisher<Purchase, EventBusConfig<Purchase>> eventPublisher)
        {
            _purchaseRepository = purchaseRepository;
            _eventPublisher = eventPublisher;
        }

        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_purchaseRepository.All());
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var item = _purchaseRepository.Get(id);

            if (item == null)
                return PurchaseNotFound(id);

            return Ok(item);
        }

        // POST api/values
        [HttpPost]
        public IActionResult Post([FromBody]Purchase value)
        {
            if (ModelState.IsValid)
            {
                var purchase = value.WithNewId();
                _eventPublisher.Publish(purchase, EventType.Set);
                return Created($"{Request.Path.Value.TrimEnd('/')}/{purchase.Id}", purchase);
            }

            return BadRequest(ModelState);
        }

        private IActionResult PurchaseNotFound(string id)
        {
            return NotFound(new { Id = id, Message = $"Failed to locate purchase with id {id}." });
        }
    }
}
