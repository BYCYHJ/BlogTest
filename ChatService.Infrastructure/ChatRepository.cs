using ChatService.Domain;
using ChatService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using RedisHelper;
using SqlHelper;
using System.ComponentModel;

namespace ChatService.Infrastructure
{
    public class ChatRepository : IChatRepository
    {
        private readonly ChatServiceDbContext _chatDbContext;
        private readonly IRedisListHelper _redisHelper;

        public ChatRepository(ChatServiceDbContext chatDbContext,IRedisListHelper redisHelper)
        {
            _chatDbContext = chatDbContext;
            _redisHelper = redisHelper;
        }

        public async Task CreateTableAsync(string name)
        {
            string actualName = "_"+name;
            int result = _chatDbContext.Database.SqlQuery<int>($"SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = {actualName}").Count();
            //结果不为1说明没有该表
            if (result != 1)
            {
                //创建表
                //sql语句
                string sql =  BaiSqlHelper.CreateTableSql<Message>(actualName);
                await _chatDbContext.Database.ExecuteSqlRawAsync(sql);
                _chatDbContext.SaveChanges();
            }
        }

        public async Task DeleteRedisKeyAsync(string key)
        {
            await _redisHelper.DeleteKeyAsync(key);
        }

        public IEnumerable<Message> GetSqlMessages(string key, int count, bool inverse = true)
        {
            string isInverse = inverse ? "DESC" : "ASC";
            var messages  = _chatDbContext.Messages.FromSql($"SELECT * FROM {key} ORDER BY SendTime {isInverse} LIMIT {count}").ToList();
            return messages;
        }

        public async Task<IEnumerable<string>> GetPatternKeys(string pattern, int scanSize)
        {
            return await _redisHelper.GetPatternKeysAsync(pattern, scanSize);
        }

        public async Task UploadMsgToRedisAsync(IEnumerable<Message> messageList, string key)
        {
            await _redisHelper.ListSetAsync(key,messageList);
        }

        public async Task UploadMsgToRedisAsync(Message message, string key)
        {
            await _redisHelper.SingleSetAsync(key,message);
        }

        public async Task UploadMsgToSqlAsync(IEnumerable<Message> messages, string tableName)
        {
            string sql = BaiSqlHelper.BulkInsertSql(datum:messages,tableName:"_"+tableName);
            await _chatDbContext.Database.ExecuteSqlRawAsync(sql);
            _chatDbContext.SaveChanges();
        }

        public async Task<IEnumerable<Message>> GetRedisMessage(string key)
        {
            return await _redisHelper.ListGetAsync<Message>(key);
        }

        public async Task UpdateMsgAsync(Message message, string tableName)
        {
            string sql = BaiSqlHelper.UpdateSql(message,tableName);
            await _chatDbContext.Database.ExecuteSqlRawAsync(sql);
            _chatDbContext.SaveChanges();
        }
    }
}
