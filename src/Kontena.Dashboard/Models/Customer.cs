using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using CSharpVitamins;
using Kontena.EventSourcing;

namespace Kontena.Dashboard.Models
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
    }
}