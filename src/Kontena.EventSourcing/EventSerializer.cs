using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Kontena.EventSourcing
{
    public class EventSerializer<T>
        where T : IEventPayload
    {
        private readonly JsonSerializerSettings _settings;

        public EventSerializer()
        {
            _settings = new JsonSerializerSettings();
            _settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            _settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
        }
        public string Serialize(Event<T> evt)
        {
            return JsonConvert.SerializeObject(evt, _settings);
        }

        public Event<T> Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<Event<T>>(json, _settings);
        }
    }
}