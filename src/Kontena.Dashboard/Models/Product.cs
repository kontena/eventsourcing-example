using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.Dashboard.Models
{
    public class Product
        : IEventPayload
    {
        public string Id { get; }

        [Required]
        public string Name { get; }

        [Required]
        public decimal Price { get; }

        public Product(
            string id,
            string name,
            decimal price
        )
        {
            Id = id;
            Name = name;
            Price = price;
        }
    }
}