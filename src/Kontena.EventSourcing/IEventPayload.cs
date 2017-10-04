using System;

namespace Kontena.EventSourcing
{
    public interface IEventPayload
    {
        string Id { get; }
    }
}