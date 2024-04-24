using BlogDomainCommons;

namespace BlogService.Domain.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; private set; }//博客标题
        public string Content { get; private set; }//文章内容
        public List<TagClass> Tags { get; set; } = new List<TagClass>();//文章标签
        public Guid UserId { get; init; }//作者
        public int StartCount { get; private set; } = 0;//点赞数
        public string? PreviewPhoto { get; private set; }//博客预览图
        public List<Comment> Comments { get; set; } = new List<Comment>();//评论


        private Blog()
        {
        }

        public Blog(string title, string content, Guid userId,List<TagClass>? tags = null,string? previewPhoto=null)
        {
            Title = title;
            Content = content;
            UserId = userId;
            if(tags is not null && tags.Any())
            {
                this.Tags.AddRange(tags);
            }
            else
            {
                this.Tags.Add(TagClass.All);//为空则自动设置为All标签
            }
            PreviewPhoto = previewPhoto;
        }

        //修改博客
        public void ChangeBlog(Blog blog)
        {
            this.ChangeBlog(blog.Title,blog.Content,blog.Tags);
        }

        public void ChangeBlog(string title,string content,List<TagClass>? tags)
        {
            this.Title = title;
            this.Content = content;
            if(tags is not null && tags.Any())
            {
                this.Tags = tags;
            }
            else
            {
                this.Tags = new List<TagClass>();
            }
        }

        public void ChangeBlog(string content)
        {
            this.Content = content;
        }

        //增加点赞数
        public void AddStar()
        {
            StartCount++;
        }

        public void RemoveStars()
        {
            if(this.Tags.Count > 0)
            {
                StartCount--;
            }
            StartCount = 0;
        }

        //修改预览图
        public void UpdatePreviewPhoto(string url)
        {
            PreviewPhoto = url;
        }
    }
}
