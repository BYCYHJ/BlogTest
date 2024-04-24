using ChatService.Domain;
using ChatService.Domain.Dtos;
using ChatService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace ChatService.WebApi.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ChatDomainService _chatDomainService;
        private readonly ConnectionMapping<string> _connectionMapping;
        private readonly IMemoryCache _memoryCache;

        public MessageHub(ChatDomainService chatDomainService,ConnectionMapping<string> connectionMapping,IMemoryCache memoryCache)
        {
            _chatDomainService = chatDomainService;
            _connectionMapping = connectionMapping;
            _memoryCache = memoryCache;
        }

        //连接时将userId与当前connectionId绑定
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.GetHttpContext()!.Request.Query["userId"];
            if (userId != null)
            {
                //绑定userid-connectionId映射表
                _connectionMapping.Add(userId, Context.ConnectionId);
                //将用户添加进组，省去内存映射，且防止connectionId变化问题
                //此时一个userId就是一个单独的组，组中为所有的connectionId,可以进行多设备聊天同步
                await Groups.AddToGroupAsync(Context.ConnectionId, userId);
                //查看是否含有未读消息
                if(_memoryCache.TryGetValue(userId, out List<string>? senders))
                {
                    //满足说明含有未读消息
                    if(senders != null && senders.Count > 0)
                    {
                        //向客户端发送消息
                        string tableJson = JsonConvert.SerializeObject(senders);
                        await Clients.Group(userId).SendAsync("getHaventRead",tableJson);
                        //将该条信息消费
                        _memoryCache.Remove(userId);
                    }
                }
            }
            await base.OnConnectedAsync();
        }

        //连接断开后取消connectionId的绑定
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userId = _connectionMapping.GetUser(Context.ConnectionId)!;
            await Groups.RemoveFromGroupAsync(Context.ConnectionId,userId);
            _connectionMapping.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task RemoveOne(string userId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
        }

        /// <summary>
        /// 单聊发送消息
        /// </summary>
        /// <param name="messageJson"></param>
        /// <returns></returns>
        public async Task SendMessage(string messageJson) {
            Message? message = JsonConvert.DeserializeObject<Message>(messageJson);
            if (message == null) return;
            //将发送者和接收者的guid的hash作为表名
            var tableName = _chatDomainService.CreateTableName(message.SenderId, message.RecipientId);
            //如果用户处于离线状态
            if (_connectionMapping.GetConnections(message.RecipientId).Count() == 0)
            {
                //将消息改为未读
                message.SetHaveRead(false);
                //缓存消息到redis中
                await _chatDomainService.InsertToRedis(tableName, message);
                //将未读状态放到缓存中
                if (!_memoryCache.TryGetValue(message.RecipientId,out List<string>? senders))
                {
                    senders = new List<string>();
                }
                senders!.Add(message.SenderId);
                _memoryCache.Set(message.RecipientId, senders);
            }
            else
            {
                message.SetHaveRead(true);
                //缓存消息到redis中
                await _chatDomainService.InsertToRedis(tableName, message);
                //发送给接收者
                await Clients.Group(message.RecipientId).SendAsync("ReceiveMessage",JsonConvert.SerializeObject(message));
            }
        }

        /// <summary>
        /// 将发送来的信息全部变为已读
        /// </summary>
        /// <param name="messagesJson"></param>
        /// <returns></returns>
        public async Task SetMsgRead(string messagesJson)
        {
            List<Message> messages = JsonConvert.DeserializeObject<List<Message>>(messagesJson)!;
            if(messages == null || messages.Count == 0) return;
            //获取表名
            var firstMsg = messages.FirstOrDefault();
            string tableName = _chatDomainService.CreateTableName(firstMsg!.SenderId,firstMsg.RecipientId);
            await _chatDomainService.SetMsgHaveRead(messages, tableName);
        }

        /// <summary>
        /// 推送通知
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task NotifyOne(string userId,Notification notification)
        {
            //查询是否在线
            int count =_connectionMapping.GetConnections(userId).Count();
            //如果不在线就不进行推送
            if (count <= 0) return;
            //推送通知
            await Clients.Group(userId).SendAsync("ReceiveMsg",JsonConvert.SerializeObject(notification));
        }
    }
}
