using System;
using CSharpVitamins;

namespace Kontena.EventSourcing
{
    public class EventBusConfig<TPayload>
        : IEventBusConfig
    {
        private static readonly Type _type = typeof(TPayload);
        private static readonly ShortGuid _groupid = ShortGuid.NewGuid();

        public string BrokerList
        {
            get { return Environment.GetEnvironmentVariable("KAFKA_BROKERS"); }
        }

        public string Topic
        {
            get { return $"eventbus.{_type.Name.ToLower()}s"; }
        }

        public string GroupId
        {
            get { return $"{_type.Namespace.ToLower()}.{_groupid}"; }
        }
    }
}