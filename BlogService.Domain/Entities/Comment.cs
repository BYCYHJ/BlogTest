using BlogDomainCommons;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public required string Content { get; init; }//评论内容
        public required Guid UserId { get; init; }//发表评论的用户Id
        public int StarCount { get; private set; } = 0;//点赞数
        public required Guid BlogId { get; init; }//所属博客的id
        public Blog? Blog { get; private set; }//所属博客(引用属性)
        public List<Comment>? ChildrenComments { get;set; }//评论所包含的评论
        public Guid? ParentId { get; init; }//所属的评论id
        public Comment? ParentComment { get; private set; }//所属的评论(引用属性)

        private Comment() { }

        public Comment(string content,string userId,string blogId,string? parentId = null)
        {
            this.Content = content;
            this.UserId = Guid.Parse(userId);
            this.UserId = Guid.Parse(blogId);
            this.ParentId = parentId is null ? null : Guid.Parse(parentId);
        }

        public void AddStar()
        {
            this.StarCount++;
        }

        public void RemoveStar()
        {
            if(this.StarCount > 0)
            {
                this.StarCount--;
            }
            this.StarCount = 0;
        }

    }
}
