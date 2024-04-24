using FileService.Domain;
using FileService.GrpcApi;
using FileService.WebApi.Protos;
using Grpc.Core;

namespace FileService.GrpcApi.Services
{
    public class FileProtoService : FileApi.FileApiBase
    {
        private readonly ServerFileService _fileService;
        public FileProtoService(ServerFileService fileService)
        {
            _fileService = fileService;
        }

        /// <summary>
        /// 批量上传图片,返回图片地址集合
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<FileBulkResponse> BulkUploadPhoto(FileBase64BulkRequest request, ServerCallContext context)
        {
            try
            {
                var images = await _fileService.UploadBulkPhotoAsync(request.Route, request.ImgStrs, request.UserId, request.BlogId);
                List<FileResponse> response = images.Select(img =>
                {
                    return new FileResponse { PreviewUrl = img.previewUrl, FileUrl = img.higerUrl };
                }).ToList();
                FileBulkResponse result = new FileBulkResponse();
                result.FileUrls.AddRange(response);
                return result;
            }catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        /// <summary>
        /// 上传单个图片
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<FileResponse> UploadPhoto(FileBase64Request request, ServerCallContext context)
        {
            try
            {
                var image = await _fileService.UploadPhotoAsync(request.FileUrl, request.FileName, request.Base64Str, request.UserId, request.BlogId);
                return new FileResponse
                {
                    FileUrl = image.FileUrl,
                    PreviewUrl = image.PreviewPhoto
                };
            }catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal,ex.Message));
            }
        }
    }
}
