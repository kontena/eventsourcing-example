using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Microsoft.Extensions.Logging;

namespace Kontena.EventSourcing
{
    public class EventPublisher<TPayload, TConfig> : IDisposable
        where TPayload : IEventPayload
        where TConfig : IEventBusConfig
    {
        protected readonly TConfig _config;
        protected readonly Producer<string, string> _producer;
        protected readonly EventSerializer<TPayload> _eventSerializer;
        protected readonly ILogger<EventSubscriber<TPayload, TConfig>> _logger;

        public EventPublisher(
            TConfig config,
            EventSerializer<TPayload> eventSerializer,
            ILogger<EventSubscriber<TPayload, TConfig>> logger)
        {
            _config = config;
            _eventSerializer = eventSerializer;
            _logger = logger;

            var kafkaConfig = new Dictionary<string, object>
            {
                { "bootstrap.servers", config.BrokerList }
            };

            var serializer = new StringSerializer(Encoding.UTF8);
            _producer = new Producer<string, string>(kafkaConfig, serializer, serializer);
        }

        public Task Publish(TPayload payload, EventType type)
        {
            var json = _eventSerializer.Serialize( new Event<TPayload> { EventType = type, Payload = payload } );
            _logger.LogInformation($"Publishing message {json}");

            return _producer.ProduceAsync(_config.Topic, payload.Id, json);
        }

        public void Dispose()
        {
            _producer.Dispose();
        }
    }
}