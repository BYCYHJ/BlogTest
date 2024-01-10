using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRabbitMqHelper(this IServiceCollection services, string exchangerType, string queueName, params Assembly[] assemblies)
        {
            return AddRabbitMqHelper(services,exchangerType,queueName,assemblies.ToList());
        }

        public static IServiceCollection AddRabbitMqHelper(this IServiceCollection services, string exchangerType, string queueName, IEnumerable<Assembly> assemblies)
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in assemblies)
            {
                var handlerTypes = assembly.GetTypes().Where(t => !t.IsAbstract && t.IsAssignableTo(typeof(IEventHandler))).ToList<Type>();
                if (handlerTypes.Any())
                {
                    types.AddRange(handlerTypes);
                }
            }
            return AddRabbitMqHelper(services,exchangerType,queueName:queueName,types);
        }

        public static IServiceCollection AddRabbitMqHelper(this IServiceCollection services,string exchangerType,string queueName, IEnumerable<Type> handlerTypes)
        {
            //将所有handler注册
            foreach (var handlerType in handlerTypes)
            {
                services.AddScoped(handlerType, handlerType);
            }
            services.AddSingleton<IEventBus>(sp =>
            {
                RabbitMqOptions options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;
                var factory = new ConnectionFactory()
                {
                    HostName = options.HostName,
                    Port = options.Port,
                    DispatchConsumersAsync = true
                };
                if (options.UserName != null)
                {
                    factory.UserName = options.UserName;
                }
                if (options.Password != null)
                {
                    factory.Password = options.Password;
                }
                var connection = factory.CreateConnection();
                var serviceScopeFactory = sp.GetService<IServiceScopeFactory>()!;
                var eventBus = new RabbitMqEventBus(queueName: queueName, exchangeName: options.ExchangerName, connection, serviceScopeFactory);
                foreach (var handlerType in handlerTypes)
                {
                    var handlerAttrs = handlerType.GetCustomAttributes<EventNameAttribute>();
                    //若没有特性则报错
                    if (!handlerAttrs.Any())
                    {
                        throw new Exception($"No Attribute with [EventName(routingkey)]");
                    }
                    //将所包含的Event全部注册
                    foreach (var handlerAttr in handlerAttrs)
                    {
                        var eventName = handlerAttr.EventName;
                        eventBus.Subscribe(eventName, handlerType, exchangerType);
                    }
                }
                return eventBus;
            });
            return services;
        }
    }
}
