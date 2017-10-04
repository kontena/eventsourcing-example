using System;
using System.ComponentModel.DataAnnotations;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.CustomerService.Models
{
    public class Customer
        : IEventPayload
    {
        public string Id { get; }

        [Required]
        public string FirstName { get; }

        [Required]
        public string LastName { get; }

        public Customer(
            string id,
            string firstName,
            string lastName
        )
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
        }

        public Customer WithNewId()
        {
            return new Customer(
                id: CSharpVitamins.ShortGuid.NewGuid(),
                firstName: FirstName,
                lastName: LastName
            );
        }

        public Customer WithFirstName(string firstName)
        {
            return new Customer(
                id: Id,
                firstName: firstName,
                lastName: LastName
            );
        }

        public Customer WithLastName(string lastName)
        {
            return new Customer(
                id: Id,
                firstName: FirstName,
                lastName: lastName
            );
        }
    }
}