using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisHelper
{
    public interface IRedisService
    {
        Task<bool> SetAsync(string key, string value, double expireSeconds = 0);

        Task<string?> GetAsync(string key);

    }
}
