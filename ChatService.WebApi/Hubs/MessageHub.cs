using ChatService.Domain;
using ChatService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace ChatService.WebApi.Hubs
{
    public class MessageHub : Hub
    {
        private readonly ChatDomainService _chatDomainService;
        private readonly ConnectionMapping<string> _connectionMapping = new ConnectionMapping<string>();

        public MessageHub(ChatDomainService chatDomainService)
        {
            _chatDomainService = chatDomainService;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            string? userId = Context.GetHttpContext()!.Request.Query["userId"];
            if(userId != null)
            {
                //绑定userid
                _connectionMapping.Add(userId, Context.ConnectionId);
            }
            return base.OnDisconnectedAsync(exception);
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
            var tableName = _chatDomainService.CreateTableName(message.SenderId,message.RecipientId);
            //缓存消息到redis中
            await _chatDomainService.UpdateToRedis(tableName,message);
            //发送给接收者
            await Clients.User(message.RecipientId).SendAsync("ReceiveMessage",JsonConvert.SerializeObject(message));
        }
    }
}
