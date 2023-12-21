
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace RedisHelper
{
    public class RedisService : IRedisService
    {
        //private readonly ConnectionMultiplexer _multiplexer;
        private readonly IDistributedCache _cache;

        //public RedisService(string conStr)
        //{
        //    _multiplexer = ConnectionMultiplexer.Connect(conStr);
        //}
        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        
        /// <summary>
        /// 通过key获取value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<string?> GetAsync(string key)
        {
            //var db = _multiplexer.GetDatabase();
            //return await db.StringGetAsync(key);
             return await _cache.GetStringAsync(key);
        }

        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireSeconds"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task SetAsync(string key, string value, double expireSeconds = 0)
        {
            //var db = _multiplexer.GetDatabase();
            if(expireSeconds == 0)
            {
                //return await db.StringSetAsync(key,value);
                await _cache.SetStringAsync(key, value);
            }
            var options = new DistributedCacheEntryOptions();
            options.SetAbsoluteExpiration(TimeSpan.FromSeconds(expireSeconds));
            await _cache.SetStringAsync(key,value,options);
        }
    }
}
