using BlogService.Domain.Entities;

namespace BlogService.WebApi.Controllers
{
    public record BlogRequest(string Title,string Content,List<TagClass>? Tags,string? UserId=null,string? PreviewStr=null);
    //public record FileBase64BulkRequest(string route, IEnumerable<string> imgStrs, string? userId = null, string? blogId = null);
    //public record FileResponse(Uri? previewUrl, Uri filesUrl);
    //public record FileBase64Request(string route,string fileName,string imgStrs, string? userId = null, string? blogId = null);
}
