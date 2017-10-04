using System;

namespace Kontena.EventSourcing
{
    public class ReceivedRawEventArgs
    {
        public string Key { get; }
        public string Event { get; }

        public ReceivedRawEventArgs(string key, string evt)
        {
            Key = key;
            Event = evt;
        }
    }
}