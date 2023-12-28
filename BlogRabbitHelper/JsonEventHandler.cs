using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    //将handler的data变为任意类型
    public abstract class JsonEventHandler<T> : IEventHandler
    {
        public Task Handle(string eventName, string eventData)
        {
            T? data = JsonSerializer.Deserialize<T?>(eventData);
            return JsonHandle(eventName,data);
        }

        public abstract Task JsonHandle(string eventName, T? eventData);
    }
}
