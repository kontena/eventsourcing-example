using System;
using System.Collections.Generic;
using System.Linq;
using Kontena.EventSourcing;
using Kontena.Dashboard.Models;

namespace Kontena.Dashboard.Services
{
    public class Repository
    {
        private readonly IRepository<Customer> _customers;
        private readonly IRepository<Product> _products;
        private readonly IRepository<Purchase> _purchases;

        public Repository(
            IRepository<Customer> customers,
            IRepository<Product> products,
            IRepository<Purchase> purchases
        )
        {
            _customers = customers;
            _products = products;
            _purchases = purchases;
        }

        public IEnumerable<CustomerPurchases> GetCustomersWithPurchases(string customerId = null)
        {
            return _customers.
                All().
                Where(customer => customerId == null || customer.Id == customerId).
                Select(customer => new CustomerPurchases(
                    id: customer.Id,
                    firstName: customer.FirstName,
                    lastName: customer.LastName,
                    purchaseCount: _purchases.All().Where(purchase => purchase.CustomerId == customer.Id).
                                                    Count(),
                    totalSpent: _purchases.All().Where(purchase => purchase.CustomerId == customer.Id).
                                                Select(purchase => purchase.Total).
                                                Sum()
                )).ToList();
        }

        public IEnumerable<CustomerPurchase> GetCustomerPurchasesForCustomer(string customerId)
        {
            var customer = _customers.Get(customerId);

            if (customer == null)
            {
                return Enumerable.Empty<CustomerPurchase>();
            }

            return _purchases.
                All().
                Where(purchase => purchase.CustomerId == customer.Id).
                Select(purchase => new CustomerPurchase(
                    customer: customer,
                    product: _products.Get(purchase.ProductId),
                    amountSpent: purchase.Total,
                    transactionDate: purchase.TransactionDate
                )).ToList();
        }

        public IEnumerable<ProductPurchases> GetProductsWithPurchases(string productId = null)
        {
            return _products.
                All().
                Where(product => productId == null || product.Id == productId).
                Select(product => new ProductPurchases(
                    id: product.Id,
                    name: product.Name,
                    price: product.Price,
                    purchaseCount: _purchases.All().Where(purchase => purchase.ProductId == product.Id).
                                                    Count(),
                    customerCount: _purchases.All().Where(purchase => purchase.ProductId == product.Id).
                                                    Select(purchase => purchase.CustomerId).
                                                    Distinct().
                                                    Count(),
                    totalSpent: _purchases.All().Where(purchase => purchase.ProductId == product.Id).
                                                Select(purchase => purchase.Total).
                                                Sum()
                )).ToList();
        }

        public IEnumerable<CustomerPurchase> GetCustomerPurchasesForProduct(string productId)
        {
            var product = _products.Get(productId);

            if (product == null)
            {
                return Enumerable.Empty<CustomerPurchase>();
            }

            return _purchases.
                All().
                Where(purchase => purchase.ProductId == product.Id).
                Select(purchase => new CustomerPurchase(
                    customer: _customers.Get(purchase.CustomerId),
                    product: product,
                    amountSpent: purchase.Total,
                    transactionDate: purchase.TransactionDate
                )).ToList();
        }
    }
}