using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Kontena.EventSourcing
{
    public class PersistingEventSubscriber<TPayload, TConfig> : EventSubscriber<TPayload, TConfig>
        where TPayload : IEventPayload
        where TConfig : IEventBusConfig
    {
        protected readonly IRepository<TPayload> _repository;

        public PersistingEventSubscriber(
            TConfig config,
            EventSerializer<TPayload> eventSerializer,
            IRepository<TPayload> repository,
            ILogger<EventSubscriber<TPayload, TConfig>> logger)
            : base(config, eventSerializer, logger)
        {
            _repository = repository;
        }

        protected override void OnValidEventReceived(ReceivedEventArgs<TPayload> args)
        {
            switch(args.Event.EventType)
            {
                case EventType.Set:
                    _repository.Set(args.Event.Payload);
                    break;
                case EventType.Remove:
                    _repository.Remove(args.Event.Payload.Id);
                    break;
            }

            base.OnValidEventReceived(args);
        }
    }
}