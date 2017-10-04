using System;

namespace Kontena.EventSourcing
{
    public class ReceivedEventArgs<T>
        where T : IEventPayload
    {
        public Event<T> Event { get; }

        public ReceivedEventArgs(Event<T> evt)
        {
            Event = evt;
        }
    }
}