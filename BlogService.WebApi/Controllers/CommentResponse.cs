namespace BlogService.WebApi.Controllers
{
    public record CommentResponse(string content, string blogId, string? userId = null, string? parentId = null);
}
