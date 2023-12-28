using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    public interface IEventBus
    {
        void Publish<T>(string eventName, T? data);
        void Subscribe(string eventName, Type handlerType,string exchangerType);
    }
}
