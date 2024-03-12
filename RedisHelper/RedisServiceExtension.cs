using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisHelper
{
    public static class RedisServiceExtension
    {
        public static IServiceCollection AddRedisListHelper(this IServiceCollection services,string conStr,int defaultDb=0)
        {
            //建立连接
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(conStr);
            //注册RedisListHelper
            services.AddSingleton<IRedisListHelper, RedisListHelper>(s =>
            {
                RedisListHelper redisListHelper = new RedisListHelper(connection, defaultDb);
                return redisListHelper;
            });
            return services;
        }
    }
}
