
using StackExchange.Redis;

namespace RedisHelper
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _multiplexer;
        public RedisService(string conStr)
        {
            _multiplexer = ConnectionMultiplexer.Connect(conStr);
        }
        
        /// <summary>
        /// 通过key获取value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string?> GetAsync(string key)
        {
            var db = _multiplexer.GetDatabase();
            return await db.StringGetAsync(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> SetAsync(string key, string value, double expireSeconds = 0)
        {
            var db = _multiplexer.GetDatabase();
            if(expireSeconds == 0)
            {
                return await db.StringSetAsync(key,value);
            }
            return await db.StringSetAsync(key,value,TimeSpan.FromSeconds(expireSeconds));
        }
    }
}
