using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =true)]
    public class EventNameAttribute :Attribute
    {
        public string EventName {  get; set; }
        public EventNameAttribute(string name)
        {
            EventName = name;
        }
    }
}
