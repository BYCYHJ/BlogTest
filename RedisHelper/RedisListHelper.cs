using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisHelper
{
    public class RedisListHelper : IRedisListHelper
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly int _defaultDb = 0;
        public RedisListHelper(ConnectionMultiplexer connectionMultiplexer, int defaultDb = 0)
        {
            _connectionMultiplexer = connectionMultiplexer;
            _defaultDb = defaultDb;
        }

        private IDatabase GetDatabase()
        {
            return _connectionMultiplexer.GetDatabase(_defaultDb);
        }

        /// <summary>
        /// 获取列表所有元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<T>> ListGetAsync<T>(string key)
        {
            var data = await GetDatabase().ListRangeAsync(key);
            var result = new List<T>();
            foreach (var item in data)
            {
                result.Add(JsonConvert.DeserializeObject<T>(item!)!);
            }
            return result;
        }

        /// <summary>
        /// 将多个新元素添加到列表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ListSetAsync<T>(string key, IEnumerable<T> list)
        {
            IDatabase database = GetDatabase();
            foreach (var item in list)
            {
                string json = JsonConvert.SerializeObject(item);
                await database.ListRightPushAsync(key,json);
            }
        }

        /// <summary>
        /// 将单个新元素插入到列表末尾
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task SingleSetAsync<T>(string key, T data)
        {
            IDatabase db = GetDatabase();
            await db.ListRightPushAsync(key,JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task DeleteKeyAsync(string key)
        {
            IDatabase db = GetDatabase();
            await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 按照指定字符串检索匹配字符串的key
        /// </summary>
        /// <param name="pattern">匹配规则</param>
        /// <param name="scanSize">每次检索的大小</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<IEnumerable<string>> GetPatternKeysAsync(string pattern, int scanSize=1000)
        {
            IDatabase db = GetDatabase();
            var cursor = 0;
            var patternKeys = new List<string>();
            while (true)
            {
                var scanResult = await db.ExecuteAsync("SCAN",cursor.ToString(),"COUNT",scanSize.ToString());
                RedisResult[]? keys = (RedisResult[]?)scanResult[1];
                if(keys == null)
                {
                    break;
                }
                foreach (var key in keys)
                {
                    patternKeys.Add(key.ToString());
                }
                cursor = int.Parse(scanResult[0].ToString());
                if(cursor == 0)
                {
                    break;
                }
            }
            return patternKeys;
        }
    }
}
