using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain.Dtos
{
    public class Notification 
    {
        public NotificationType notificationType { get; init; }//推送消息的类型
        public string message { get; private set; }

        public Notification(NotificationType notificationType,string message)
        {
            this.notificationType = notificationType;
            this.message = message;
        }

        public void ChangeMsg(string msg)
        {
            this.message = msg;
        }
    }

    public enum NotificationType
    {
        Message =1,//信息
        Favorite = 2,//收藏
        Subscribe = 3,//关注
        Like = 4,// 喜欢(点赞)
    }
}
