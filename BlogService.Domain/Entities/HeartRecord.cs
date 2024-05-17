using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain.Entities
{
    public class HeartRecord
    {
        public string UserId { get; init; }//用户id
        public string ObjectId { get; init; }//博客或者评论id
        public HeartType Type { get; init; }//类型 0：博客，1：评论
        public DateTime CreateOnTime { get; init; }//创建时间
        public bool HaveRead { get;private set; }//该条订阅是否已读(对于被订阅者)

        public HeartRecord() { }
        public HeartRecord(string userId, string objectId, HeartType type)
        {
            UserId = userId;
            ObjectId = objectId;
            Type = type;
            CreateOnTime = DateTime.Now;
            HaveRead = false;
        }

        /// <summary>
        /// 设置为已读
        /// </summary>
        public void setRead()
        {
            this.HaveRead = true;
        }
    }

    public enum HeartType {
        Blog = 0,
        Comment = 1,
    }
}
