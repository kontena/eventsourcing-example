using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.Dashboard.Models
{
    public class CustomerPurchase
    {
        public Customer Customer { get; }
        public Product Product { get; }
        public decimal AmountSpent { get; }
        public DateTime TransactionDate { get; }

        public CustomerPurchase(
            Customer customer,
            Product product,
            decimal amountSpent,
            DateTime transactionDate
        )
        {
            Customer = customer;
            Product = product;
            AmountSpent = amountSpent;
            TransactionDate = transactionDate;
        }
    }
}