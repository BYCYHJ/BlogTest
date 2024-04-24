using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain
{
    public class ConnectionMapping<T> where T : notnull
    {
        private readonly Dictionary<T, HashSet<string>> _connections = new Dictionary<T, HashSet<string>>();

        /// <summary>
        /// 添加新连接映射
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionId"></param>
        public void Add(T key,string connectionId)
        {
            lock (_connections) {
                //如果不存在
                if (!_connections.TryGetValue(key,out var connections))
                {
                    //创建新的HashSet当作value
                    connections = new HashSet<string>();
                    connections.Add(connectionId);
                    _connections.Add(key, connections);
                }
                //如果存在则直接在HashSet中添加
                else
                {
                    lock (_connections)
                    {
                        _connections[key].Add(connectionId);
                    }
                }
            }
        }

        /// <summary>
        /// 移除连接映射
        /// </summary>
        /// <param name="key"></param>
        /// <param name="connectionId"></param>
        public void Remove(T key,string connectionId)
        {
            lock(_connections)
            {
                //没有则直接返回
                if(!_connections.TryGetValue(key, out var connections))
                {
                    return;
                }
                else
                {
                    lock (connections)
                    {
                        connections.Remove(connectionId);
                        //如果该user已经无任何连接，则直接删除key
                        if (connections.Count == 0)
                        {
                            _connections.Remove(key);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 无userId情况下移除connectionId绑定
        /// </summary>
        /// <param name="connectionId"></param>
        public void Remove(string connectionId)
        {
            T? userId = GetUser(connectionId);
            if (userId == null) return;
            lock (_connections)
            {
                _connections[userId].Remove(connectionId);
            }
        }

        /// <summary>
        /// 获取该用户的所有连接
        /// </summary>
        /// <param name="key"></param>
        /// <returns>以集合形式返回该用户的连接</returns>
        public IEnumerable<string> GetConnections(T key)
        {
            if(_connections.TryGetValue(key, out var connections)) return connections;
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// 查找connectionId对应的user
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public T? GetUser(string connectionId)
        {
            foreach(var connections in _connections)
            {
                if(connections.Value.Contains(connectionId)) return connections.Key;
            }
            return default(T);
        }
    }
}
