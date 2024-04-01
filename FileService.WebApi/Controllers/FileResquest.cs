namespace FileService.WebApi.Controllers
{
    public record FileBase64Resquest(string fileUrl,string fileName,string base64Str,string? userId = null);

    public record FileFormRequest(IFormFile file);
}
