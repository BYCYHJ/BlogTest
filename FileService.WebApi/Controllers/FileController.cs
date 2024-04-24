using FileService.Domain;
using FileService.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

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

        /// <summary>
        /// 上传base64图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileResponse> UploadBase64FileAsync([FromBody]FileBase64Resquest file)
        {
            var sFile =  await _fileService.UploadPhotoAsync(file.fileUrl, file.fileName,file.base64Str,file.userId,file.blogId) ;
            return new(sFile.PreviewPhoto == null ? null : new Uri(sFile.PreviewPhoto), new Uri(sFile.FileUrl));
        }

        /// <summary>
        /// 利用form上传图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<FileResponse> UploadFormFileAsync([FromForm]FileFormRequest request)
        {
            var file = request.file;
            var sFile = await _fileService.UploadPhotoAsync("/mydata/PlatformResource/",file.FileName, file);
            return new(sFile.PreviewPhoto == null ? null : new Uri(sFile.PreviewPhoto), new Uri(sFile.FileUrl));
        }

        /// <summary>
        /// 批量上传base64图片
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IEnumerable<FileResponse>> UploadBulkBase64FileAsync(FileBase64BulkRequest request)
        {
            var images = await _fileService.UploadBulkPhotoAsync(request.route, request.imgStrs, request.userId, request.blogId);
            return images.Select(img =>
            {
                return new FileResponse(img.previewUrl == null ? null : new Uri(img.previewUrl), new Uri(img.higerUrl));
            })!;
        }
    }
}
