using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.Dashboard.Models
{
    public class ProductPurchases
    {
        public string Id { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int PurchaseCount { get; }
        public int CustomerCount { get; }
        public decimal TotalSpent { get; }

        public ProductPurchases(
            string id,
            string name,
            decimal price,
            int purchaseCount,
            int customerCount,
            decimal totalSpent
        )
        {
            Id = id;
            Name = name;
            Price = price;
            PurchaseCount = purchaseCount;
            CustomerCount = customerCount;
            TotalSpent = totalSpent;
        }
    }
}