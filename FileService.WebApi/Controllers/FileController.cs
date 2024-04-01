using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileService.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ServerFileService _fileService;

        public FileController(ServerFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        public async Task<Uri> UploadBase64FileAsync([FromBody]FileBase64Resquest file)
        {
            var sFile =  await _fileService.UploadPhotoAsync(file.fileUrl, file.fileName,file.base64Str,file.userId) ;
            return new Uri(sFile.FileUrl);
        }

        [HttpPost]
        public async Task<Uri> UploadFormFileAsync([FromForm]FileFormRequest request)
        {
            var file = request.file;
            var sFile = await _fileService.UploadPhotoAsync("D:/test/",file.FileName, file);
            return new Uri(sFile.FileUrl);
        }

    }
}
