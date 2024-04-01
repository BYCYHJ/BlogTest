using ChatService.Domain;
using ChatService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlHelper;

namespace ChatService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ChatDomainService _domainService;

        public TestController(ChatDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet]
        public async Task RedisToSqlTest()
        {
            Message msgTest = new Message(sendMsg:"test") { 
                Type=MessageType.Text,
                SenderId="aaa",
                RecipientId="bbb"
            };
            string key = "test";
            await _domainService.InsertToRedis(key, msgTest);
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
