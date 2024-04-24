namespace FileService.WebApi.Controllers
{
    public record FileBase64Resquest(string fileUrl,string fileName,string base64Str,string? userId = null,string? blogId = null);

    public record FileFormRequest(IFormFile file);

    public record FileBase64BulkRequest(string route,IEnumerable<string> imgStrs,string? userId=null,string? blogId=null);
}
