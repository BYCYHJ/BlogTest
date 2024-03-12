using ChatService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain
{
    public interface IChatRepository
    {
        /// <summary>
        /// 获取mysql数据库中指定条数的信息
        /// </summary>
        /// <param name="key">消息对话名称</param>
        /// <param name="count">数量</param>
        /// <param name="inverse">是否倒序，默认true</param>
        /// <returns></returns>
        IEnumerable<Message> GetSqlMessages(string key,int count,bool inverse=true);

        /// <summary>
        /// 获取redis数据库中指定key的Message数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<IEnumerable<Message>> GetRedisMessage(string key);

        /// <summary>
        /// 上传信息到mysql做持久化处理
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="tableName">消息对话名称(数据库名称)</param>
        /// <returns></returns>
        Task UploadMsgToSqlAsync(IEnumerable<Message> messages,string tableName);

        /// <summary>
        /// 批量上传至redis
        /// </summary>
        /// <param name="messageList"></param>
        /// <param name="key">消息对话名称</param>
        /// <returns></returns>
        Task UploadMsgToRedisAsync(IEnumerable<Message> messageList,string key);

        /// <summary>
        /// 单条上传至redis
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task UploadMsgToRedisAsync(Message message, string key);

        /// <summary>
        /// 创建数据库表
        /// </summary>
        /// <param name="name">新表名</param>
        /// <returns></returns>
        Task CreateTableAsync(string name);

        /// <summary>
        /// 删除redis中的指定key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DeleteRedisKeyAsync(string key);

        /// <summary>
        /// 按照指定字符串检索匹配字符串的key
        /// </summary>
        /// <param name="pattern">普配规则</param>
        /// <param name="scanSize">每次检索的大小</param>
        /// <returns>符合条件的key集合</returns>
        Task<IEnumerable<string>> GetPatternKeys(string pattern,int scanSize);

    }
}
