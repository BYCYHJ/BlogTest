using ChatService.Domain;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.WebApi.Hubs
{
    public class ChatUserIdProvider : IUserIdProvider
    {
        /// <summary>
        /// 自定义userId提供类，根据userId与connectionId的映射提供userId
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        public string? GetUserId(HubConnectionContext connection)
        {
            string connectionId = connection.ConnectionId;
            ConnectionMapping<string> mapping = new ConnectionMapping<string>();
            string userId = mapping.GetUser(connectionId)!;
            return userId;
        }
    }
}
