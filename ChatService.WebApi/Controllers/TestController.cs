using ChatService.Domain;
using ChatService.Domain.Dtos;
using ChatService.Domain.Entities;
using ChatService.WebApi.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SqlHelper;

namespace ChatService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ChatDomainService _domainService;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ConnectionMapping<string> _connectionMapping;

        public TestController(ChatDomainService domainService,IHubContext<MessageHub> hubContext, ConnectionMapping<string> connectionMapping)
        {
            _domainService = domainService;
            _hubContext = hubContext;
            _connectionMapping = connectionMapping;
        }

        [HttpPost]
        public async Task RedisToSqlTest(string userId,Notification notification)
        {
            //查询是否在线
            int count = _connectionMapping.GetConnections(userId).Count();
            //如果不在线就不进行推送
            if (count <= 0) return;
            //推送通知
            await _hubContext.Clients.Groups(userId).SendAsync("ReceiveNotification", JsonConvert.SerializeObject(notification));
        }

        [HttpGet]
        public string GetCreateSql()
        {
            string sql = BaiSqlHelper.CreateTableSql<Message>("aaa");
            return sql;
        }

        [HttpGet]
        public string GetUpdateSql()
        {
            Message msg = new Message() { RecipientId = "aaa", Type = MessageType.Text, SenderId = "bbb" };
            return BaiSqlHelper.UpdateSql(msg,"aaa");
        }
    }
}
