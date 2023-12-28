using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    public class SubscribeManager
    {
        private Dictionary<string, List<Type>> _handlers;

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
