using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    public class SubscribeManager
    {
        //包含的所有handler
        //key为RoutingKey，即事件名称；List为订阅了该事件的所有handler的Type
        private Dictionary<string, List<Type>> _handlers = new Dictionary<string, List<Type>>();

        public void AddSubscription(string eventName,Type handlerType)
        {
            if (!HasSubscriptionForEvent(eventName))
            {
                _handlers.Add(eventName,new List<Type>());
            }
            _handlers[eventName].Add(handlerType);
        }

        public bool HasSubscriptionForEvent(string eventName) => _handlers.ContainsKey(eventName);  

        public IEnumerable<Type> GetSubscriptionForEvent(string eventName) => _handlers[eventName];
    }
}
