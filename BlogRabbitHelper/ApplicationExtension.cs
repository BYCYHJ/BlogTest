using BlogRabbitHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class ApplicationExtension
    {
        public static IApplicationBuilder UseRabbitMqHelper(this IApplicationBuilder applicationBuilder)
        {
            object? enentBus = applicationBuilder.ApplicationServices.GetService(typeof(IEventBus));
            if( enentBus == null )
            {
                throw new ArgumentNullException("没有注入IEventBus的实现类");
            }
            return applicationBuilder;
        }
    }
}
