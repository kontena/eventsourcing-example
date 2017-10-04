using System;
using System.Collections.Generic;

namespace Kontena.EventSourcing
{
    public class InMemoryRepository<TModel> : IRepository<TModel>
        where TModel : IEventPayload
    {
        private readonly Dictionary<string, TModel> _data = new Dictionary<string, TModel>();

        public IEnumerable<TModel> All()
        {
            return _data.Values;
        }

        public TModel Get(string id)
        {
            TModel model;
            _data.TryGetValue(id, out model);
            return model;
        }

        public void Set(TModel model)
        {
            _data[model.Id] = model;
        }

        public void Remove(string id)
        {
            _data.Remove(id);
        }
    }
}