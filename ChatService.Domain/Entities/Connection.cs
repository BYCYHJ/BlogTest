using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain.Entities
{
    public class Connection
    {
        public Guid ConnectionId { get; set; }
        public bool IsConnected { get; set; } = false;
        public required string UserId {  get; set; }
    }
}
