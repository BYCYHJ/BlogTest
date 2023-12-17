using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public DateTime CreateOnTime { get; init; }
        public DateTime? DeleteTime { get; private set; }//软删除时间
        public bool isDelete { get;private set; }//是否软删除

        public User(string userName) : base(userName)
        {
            this.Id = Guid.NewGuid();
            this.CreateOnTime = DateTime.Now;
        }

        //软删除
        public void SoftDelete()
        {
            this.isDelete = true;
            this.DeleteTime = DateTime.Now;
        }

    }
}
