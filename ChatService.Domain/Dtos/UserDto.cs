using ChatService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain.Dtos
{
    internal class UserDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<Connection> Connections { get; private set; } = new List<Connection>();//所包含的对话链接
        public string? AvatarUrl {  get; set; }//头像路径
        public UserDto(Guid id, string name, List<Connection>? connections = null, string? avatar = null)
        {
            Id = id;
            Name = name;
            if (connections != null)
            {
                Connections = connections;
            }
            AvatarUrl = avatar;
        }

        //新增链接
        public void AddConnection(Connection connection)
        {
            this.Connections.Add(connection);
        }
    }
}
