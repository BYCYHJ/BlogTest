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
        public string Content { get; private set; }//评论内容
        public Guid UserId { get; private set; }//发表评论的用户Id
        public int StarCount { get; private set; } = 0;//点赞数
        public Guid BlogId { get; private set; }//所属博客的id
        public Blog? Blog { get; private set; }//所属博客(引用属性)
        public List<Comment>? ChildrenComments { get;set; }//评论所包含的评论
        public Guid? ParentId { get; private set; }//所属的评论id
        public Comment? ParentComment { get; private set; }//所属的评论(引用属性)

        private Comment() { }

        public Comment(string content,string userId,string blogId,string? parentId = null)
        {
            this.Content = content;
            this.UserId = Guid.Parse(userId);
            this.BlogId = Guid.Parse(blogId);
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
