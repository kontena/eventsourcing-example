using System;
using System.ComponentModel.DataAnnotations;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.ProductService.Models
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

        public Product WithNewId()
        {
            return new Product(
                id: CSharpVitamins.ShortGuid.NewGuid(),
                name: this.Name,
                price: this.Price
            );
        }

        public Product WithName(string name)
        {
            return new Product(
                id: this.Id,
                name: name,
                price: this.Price
            );
        }

        public Product WithPrice(decimal price)
        {
            return new Product(
                id: this.Id,
                name: this.Name,
                price: price
            );
        }
    }
}