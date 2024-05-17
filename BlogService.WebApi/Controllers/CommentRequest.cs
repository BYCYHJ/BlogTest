using BlogService.Domain.Entities;

namespace BlogService.WebApi.Controllers
{
    public record CommentRequest(string content, 
        string blogId, 
        string? userId = null, 
        string? parentId = null,
        string? highestCommentId=null
        );

    public record CommentResponse(
        string id,
        string content,
        string? parentId,
        string publishDate,
        string userId,
        string userName,
        string? avatarUrl=null, 
        int starCount=0,
        string? replyUserName=null,
        string? highestCommentId= null
        );

    public record CommentWithCount(
        Comment comment,
        int count
        );
}
