using ChatService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain
{
    public class ChatDomainService
    {
        private readonly IChatRepository chatRepository;
        public ChatDomainService(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        /// <summary>
        /// 获取mysql中的指定数量信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Message> GetMySqlMsgs(string msgKey, int count)
        {
            IEnumerable<Message> messages = chatRepository.GetSqlMessages(msgKey, count);
            return messages;
        }

        /// <summary>
        /// 获取redis中的所有Message
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Message>> GetRedisMsgsAsync(string key)
        {
            return await chatRepository.GetRedisMessage(key);
        }

        /// <summary>
        /// 上传单条信息至redis
        /// </summary>
        /// <param name="msgKey"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task UpdateToRedis(string msgKey, Message msg)
        {
            await chatRepository.UploadMsgToRedisAsync(message: msg, key: msgKey);
        }

        /// <summary>
        /// 上传到mysql数据库
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="msgs"></param>
        /// <returns></returns>
        public async Task UpdateToSql(string tableName, IEnumerable<Message> msgs)
        {
            //查询该表是否存在，如果没有该表则创建一个新表
            await chatRepository.CreateTableAsync(tableName);
            //将数据插入到表中
            await chatRepository.UploadMsgToSqlAsync(tableName: tableName, messages: msgs);
        }

        /// <summary>
        /// 删除redis的指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task DeleteRedisList(string key)
        {
            await chatRepository.DeleteRedisKeyAsync(key);
        }

        /// <summary>
        /// 获取相匹配的所有key
        /// </summary>
        /// <param name="pattern">匹配字</param>
        /// <param name="size">查询规模，不影响输出结果</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetPatternKeyAsync(string pattern,int size = 1000)
        {
            return await chatRepository.GetPatternKeys(pattern, size);
        }

        /// <summary>
        /// 根据发送者和接收者的id生成表名
        /// </summary>
        /// <param name="id1"></param>
        /// <param name="id2"></param>
        /// <returns></returns>
        public string CreateTableName(string id1,string id2)
        {
            List<string> ids = new List<string> { id1,id2};
            ids.Sort();//排序，以防止出现id1_id2和id2_id1两种组合导致表名不唯一
            string idCombine = string.Join("_", ids);
            //使用MD5进行哈希作为表名
            using(MD5 md5 = MD5.Create())
            {
                byte[] idByte = Encoding.UTF8.GetBytes(idCombine);
                byte[] hashByte = md5.ComputeHash(idByte);
                StringBuilder sb = new StringBuilder();
                foreach(byte b in hashByte)
                {
                    sb.Append(b.ToString("x2"));
                }
                string tableName = sb.ToString();
                return tableName;
            }
        }
    }
}
