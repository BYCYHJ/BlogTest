using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatService.Domain.Entities
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public required MessageType Type { get; init; }//文件类型
        public required string SenderId {  get; init; }//发送者id
        public required string RecipientId {  get; init; }//接收者id
        public DateTime SendTime { get; init; }//发送时间
        public string? SendMsg {  get; init; }//发送的文本内容(可无)
        public string? DataRoute {  get; init; }//文件路径(照片、文件)(可无)

        public Message(string? sendMsg = null, string? dataRoute = null)
        {
            SendTime = DateTime.Now;
            SendMsg = sendMsg;
            DataRoute = dataRoute;
        }
    }


    /// <summary>
    /// 信息种类
    /// </summary>
    public enum MessageType
    {
        Unsepecified = 0,//未指定
        Text = 1,//文字
        Photo = 2,//照片
        Document = 3,//文件
    }
}
