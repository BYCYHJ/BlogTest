using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlogRabbitHelper
{
    public class RabbitMqEventBus : IEventBus, IDisposable
    {
        private readonly string _queueName;
        private readonly string _exchangeName;
        private readonly IConnection _connection;
        private readonly SubscribeManager _subscriptionManager;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServiceScope _serviceScope;

        public RabbitMqEventBus(string queueName, string exchangeName, IConnection connection, IServiceScopeFactory serviceProviderFactory)
        {
            _queueName = queueName;
            _exchangeName = exchangeName;
            _connection = connection;
            _subscriptionManager = new SubscribeManager();
            _serviceScope = serviceProviderFactory.CreateScope();
            _serviceProvider = _serviceScope.ServiceProvider;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 发布集成事件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        public void Publish<T>(string eventName, T? data)
        {
            using var channel = _connection.CreateModel();
            //声明交换机
            channel.ExchangeDeclare(exchange: _exchangeName,
                type: "fanout",//模式为订阅模式
                durable: true
                );
            byte[] body;
            if (data is null)
            {
                body = new byte[0];
            }
            else
            {
                var jsonData = JsonSerializer.Serialize(data);
                body = Encoding.UTF8.GetBytes(jsonData);
            }
            var property = channel.CreateBasicProperties();
            property.Persistent = true;
            channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: eventName,
                body: body,
                basicProperties: property
                );
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="handlerType"></param>
        /// <param name="exchangerType"></param>
        public void Subscribe(string eventName, Type handlerType, string exchangerType)
        {
            var consumeChannel = _connection.CreateModel();//不使用using销毁
            //交换机声明
            consumeChannel.ExchangeDeclare(
                exchange: _exchangeName,
                type: exchangerType
                );
            //队列声明
            consumeChannel.QueueDeclare(
                queue: _queueName,
                durable: true,
                exclusive: false,///是否排他, true排他的, 如果一个队列声明为排他队列, 该队列仅对首次声明它的连接可见, 并在连接断开时自动删除
                autoDelete: false
                );
            consumeChannel.QueueBind(_queueName, _exchangeName, eventName);

            _subscriptionManager.AddSubscription(eventName, handlerType);//将订阅者加入集合

            var consumer = new AsyncEventingBasicConsumer(consumeChannel);
            consumer.Received += async (sender, eventArgs) =>
            {
                var enventName = eventArgs.RoutingKey;
                var data = Encoding.UTF8.GetString(eventArgs.Body.Span);//将生产者发送的byte[] msg转为json格式的字符串
                await ProcessEvent(enventName,data);
                consumeChannel.BasicAck(eventArgs.DeliveryTag,false);//确认
            };

        }

        /// <summary>
        /// 用于消费者获取实例执行handle方法
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task ProcessEvent(string eventName,string data)
        {
            //如果有处理该事件的程序
            if (_subscriptionManager.HasSubscriptionForEvent(eventName))
            {
                //获取所有订阅了该事件的handler
                IEnumerable<Type> handlerTypes = _subscriptionManager.GetSubscriptionForEvent(eventName);
                foreach (Type handlerType in handlerTypes)
                {
                    //放在不同的scope以防DbContext共享
                    //The instance of entity type cannot be tracked because another instance
                    using var scope = _serviceProvider.CreateScope();
                    IEventHandler? handler = scope.ServiceProvider.GetService(handlerType) as IEventHandler;
                    if (handler == null)
                    {
                        throw new Exception($"未找到{handlerType}的实例");
                    }
                    await handler.Handle(eventName,data);
                }
            }
        }
    }
}
