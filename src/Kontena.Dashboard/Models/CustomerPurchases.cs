using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.Dashboard.Models
{
    public class CustomerPurchases
    {
        public string Id { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public int PurchaseCount { get; }
        public decimal TotalSpent { get; }

        public CustomerPurchases(
            string id,
            string firstName,
            string lastName,
            int purchaseCount,
            decimal totalSpent
        )
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            PurchaseCount = purchaseCount;
            TotalSpent = totalSpent;
        }
    }
}