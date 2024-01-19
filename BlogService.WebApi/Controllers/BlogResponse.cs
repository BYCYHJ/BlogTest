using BlogService.Domain.Entities;

namespace BlogService.WebApi.Controllers
{
    public record BlogResponse(string Title,string Content,List<TagClass>? Tags,string? UserId=null);
}
