using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    //将handler的data变为任意类型
    public abstract class JsonEventHandler<T> : IEventHandler
    {
        public Task Handle(string eventName, string eventData)
        {
            T? data = JsonConvert.DeserializeObject<T?>(eventData);
            return JsonHandle(eventName,data);
        }

        public abstract Task JsonHandle(string eventName, T? eventData);
    }
}
