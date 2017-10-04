using System;
using System.Collections.Generic;

namespace Kontena.EventSourcing
{
    public interface IRepository<TModel>
        where TModel : IEventPayload
    {
        IEnumerable<TModel> All();

        TModel Get(string id);

        void Set(TModel model);

        void Remove(string id);
    }
}