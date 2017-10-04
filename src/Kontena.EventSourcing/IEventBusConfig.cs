using System;

namespace Kontena.EventSourcing
{
    public interface IEventBusConfig
    {
        string BrokerList { get; }
        string Topic { get; }
        string GroupId { get; }
    }
}