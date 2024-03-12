using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisHelper
{
    public interface IRedisListHelper
    {
        Task SingleSetAsync<T>(string key,T data);
        Task ListSetAsync<T>(string key, IEnumerable<T> list);
        Task<IEnumerable<T>> ListGetAsync<T>(string key);
        Task DeleteKeyAsync(string key);
        Task<IEnumerable<string>> GetPatternKeysAsync(string pattern, int scanSize);
    }
}
