using BlogDomainCommons;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain
{
    public class Comment  : BaseEntity
    {
        public string Content {  get; init; }//评论内容
        public string BlogId { get; init; }//所属博客id
        public string UserId {  get; init; }//发表评论的用户Id
        public int StartCount {  get; private set; }//点赞数
        public bool IsReplyComment {  get; init; }//是否为回复评论的评论,0=n,1=y
        public string? ReplyCommentId {  get; init; }//所属的评论的id

        public Comment(string content,string blogId,string userId,bool isReplyComment,string replyCommentId = null) {
            Content = content;
            BlogId = blogId;
            UserId = userId;
            IsReplyComment = isReplyComment;
            if(replyCommentId is not null)
            {
                ReplyCommentId = replyCommentId;
            }
        }

    }
}
