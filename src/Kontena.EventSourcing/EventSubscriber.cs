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
    public class EventSubscriber<TPayload, TConfig>
        where TPayload : IEventPayload
        where TConfig : IEventBusConfig
    {
        public event EventHandler<ReceivedRawEventArgs> RawEventReceived;
        public event EventHandler<ReceivedEventArgs<TPayload>> ValidEventReceived;
        protected readonly TConfig _config;
        protected readonly EventSerializer<TPayload> _eventSerializer;
        protected readonly ILogger<EventSubscriber<TPayload, TConfig>> _logger;

        public EventSubscriber(
            TConfig config,
            EventSerializer<TPayload> eventSerializer,
            ILogger<EventSubscriber<TPayload, TConfig>> logger)
        {
            _config = config;
            _eventSerializer = eventSerializer;
            _logger = logger;
        }

        protected virtual void OnRawEventReceived(ReceivedRawEventArgs args)
        {
            RawEventReceived?.Invoke(this, args);
        }

        protected virtual void OnValidEventReceived(ReceivedEventArgs<TPayload> args)
        {
            ValidEventReceived?.Invoke(this, args);
        }

        public virtual Task Run()
        {
            var kafkaConfig = new Dictionary<string, object>
            {
                { "group.id", _config.GroupId },
                { "bootstrap.servers", _config.BrokerList }
            };

            return Task.Run(() =>
            {
                var typeName = typeof(TPayload).Name;

                _logger.LogInformation($"{typeName} Event Subscriber Starting");

                var serializer = new StringDeserializer(Encoding.UTF8);
                using (var consumer = new Consumer<string, string>(kafkaConfig, serializer, serializer))
                {
                    consumer.Assign(new List<TopicPartitionOffset> { new TopicPartitionOffset(_config.Topic, 0, 0) });

                    while (true)
                    {
                        Message<string, string> msg;
                        if (consumer.Consume(out msg, TimeSpan.FromSeconds(1)))
                        {
                            try
                            {
                                _logger.LogInformation($"{typeName} Event Received: {msg.Value}");
                                OnRawEventReceived(new ReceivedRawEventArgs(msg.Key, msg.Value));

                                var evt = _eventSerializer.Deserialize(msg.Value);

                                if (evt?.Validate() == true)
                                {
                                    OnValidEventReceived(new ReceivedEventArgs<TPayload>(evt));
                                }
                            }
                            catch(JsonSerializationException exc)
                            {
                                _logger.LogWarning($"{typeName} Event Error: {exc}");
                                return null;
                            }
                        }
                    }
                }

                _logger.LogInformation($"{typeName} Event Subscriber Ending");
            });
        }
    }
}