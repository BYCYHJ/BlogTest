using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    public interface IEventHandler
    {
        Task Handle(string eventName,string eventData);
    }
}
