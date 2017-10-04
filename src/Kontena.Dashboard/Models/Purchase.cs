using System;
using System.ComponentModel.DataAnnotations;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.Dashboard.Models
{
    public class Purchase
        : IEventPayload
    {
        public string Id { get; }

        [Required]
        public string ProductId { get; }

        [Required]
        public string CustomerId { get; }

        [Required]
        public decimal Total { get; }

        public DateTime TransactionDate { get; }

        public Purchase(
            string id,
            string productId,
            string customerId,
            decimal total,
            DateTime transactionDate
        )
        {
            Id = id;
            ProductId = productId;
            CustomerId = customerId;
            Total = total;
            TransactionDate = transactionDate;
        }
    }
}