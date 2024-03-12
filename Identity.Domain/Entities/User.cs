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
        public string? AvatarUrl { get; set; }//头像路径
        public DateTime CreateOnTime { get; init; }
        public DateTime? DeleteTime { get; private set; }//软删除时间
        public bool isDelete { get;private set; }//是否软删除

        public User() { }

        public User(string? userName = null,string? phoneNumber = null)
        {
            base.UserName = userName;
            this.Id = Guid.NewGuid();
            this.CreateOnTime = DateTime.Now;
            if(this.UserName == null && phoneNumber != null)//无用户名，手机登录则将用户名设为手机号，密码随机
            {
                this.PhoneNumber = phoneNumber;
                this.UserName = phoneNumber;
            }
        }

        //软删除
        public void SoftDelete()
        {
            this.isDelete = true;
            this.DeleteTime = DateTime.Now;
        }

    }
}
