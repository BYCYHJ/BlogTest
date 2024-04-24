namespace BlogService.WebApi.Controllers
{
    public record PhotoRequest(string fileName,string fileUrl,string base64Str,string? userId = null,string? blogId=null);
}
