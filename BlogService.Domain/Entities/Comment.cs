using BlogDomainCommons;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
        public string? HighestCommentId { get; init; }//所属的第一级评论Id,方便查询

        [InverseProperty(nameof(Comment.ParentComment))]
        public virtual List<Comment>? ChildrenComments { get;set; }//评论所包含的评论

        [JsonIgnore]
        [InverseProperty(nameof(Comment.ChildrenComments))]
        public virtual Comment? ParentComment { get; private set; }//所属的评论(引用属性)
        public Guid? ParentId { get; private set; }//所属的评论id



        private Comment() { }
        public Comment(string content, string blogId,string? userId=null, string? parentId = null,string? highestId = null)
        {
            this.Content = content;
            if(userId is not null)
            {
                this.UserId = Guid.Parse(userId);
            }
            else
            {
                this.UserId = Guid.Empty;
            }
            this.BlogId = Guid.Parse(blogId);
            this.ParentId = parentId is null || parentId==string.Empty ? null : Guid.Parse(parentId);
            this.HighestCommentId = highestId;
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
