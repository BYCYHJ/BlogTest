using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain.Dtos
{
    public record UserInfo(string id,string userName,string? avatarUrl);
}
