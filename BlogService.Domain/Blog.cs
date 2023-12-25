using BlogDomainCommons;

namespace BlogService.Domain
{
    public class Blog : BaseEntity
    {
        public string Title {  get;private set; }//博客标题
        public string Content { get; private set; }//文章内容
        public List<object>? Tag {  get; private set; }//文章标签
        public string UserId {  get; init; }//作者
        public int StartCount {  get; private set; }//点赞数

        public Blog(string title,string content,string userId) {
            Title = title;
            Content = content;
            UserId = userId;
        }
    }
}
