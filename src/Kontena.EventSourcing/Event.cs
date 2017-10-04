using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kontena.EventSourcing
{
    public class Event<T>
        where T : IEventPayload
    {
        public EventType EventType { get; set; }
        public T Payload { get; set; }

        public bool Validate()
        {
            if (EventType == EventType.Unknown || Payload == null)
            {
                return false;
            }

            var results = new List<ValidationResult>();
            var context = new ValidationContext(Payload, null, null);
            return Validator.TryValidateObject(Payload, context, results);
        }
    }
}