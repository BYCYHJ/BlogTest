using FileService.Domain.Entities;
using BlogHashHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace FileService.Domain
{
    public class ServerFileService
    {
        private readonly IFileRepository _repository;//数据库服务
        private readonly IFileStorage _storage;//服务器存储库

        public ServerFileService(IFileRepository repository, IFileStorage storage)
        {
            _repository = repository;
            _storage = storage;
        }

        /// <summary>
        /// 上传文件,如果已经含有则不上传
        /// </summary>
        /// <param name="route"></param>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public async Task<ServerFile> UploadPhotoAsync(string route, string fileName, string base64Str, string? userId = null, string? blogId = null)
        {
            return await UploadPhotoAsync(route, fileName, base64Str.ConvertBase64ToStream(), userId, blogId);
        }

        public async Task<ServerFile> UploadPhotoAsync(string route, string fileName, IFormFile file, string? userId = null, string? blogId = null)
        {
            return await UploadPhotoAsync(route, fileName, file.OpenReadStream(), userId, blogId);
        }

        public async Task<ServerFile> UploadPhotoAsync(string route, string fileName, Stream item, string? userId = null, string? blogId = null)
        {
            long fileSize = item.Length;
            //计算hash值，对于hash值的比较可以迅速判断文件是否重复
            string hash = HashHelper.ComputeSha256Hash(item);
            //判断服务器中是否已经含有该文件，如果有则不再上传
            ServerFile? file = await _repository.FindAsync(hash);
            if (file != null) return file;
            //判断路径是否存在，如果不存在直接创建
            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }
            //上传到文件服务器中，返回可供直接访问的url
            var urls = await _storage.UploadPhoto(route + fileName, item);
            var serverFile = ServerFile.Create(fileName, hash, urls.higherUrl, fileSize, userId, blogId,previewUrl:urls.previewUrl);
            //上传记录到数据库中
            await _repository.UploadAsync(serverFile);
            return serverFile;
        }

        /// <summary>
        /// 批量上传
        /// </summary>
        /// <param name="route"></param>
        /// <param name="base64Imgs"></param>
        /// <param name="userId"></param>
        /// <param name="blogId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<(string? previewUrl,string higerUrl)>> UploadBulkPhotoAsync(string route, IEnumerable<string> base64Imgs, string? userId = null, string? blogId = null)
        {
            List<(string previewUrl, string higerUrl)> images = new List<(string previewUrl, string higerUrl)>();
            foreach (var imageStr in base64Imgs)
            {
                string fileName = Guid.NewGuid().ToString() + ".png";
                Stream stream = imageStr.ConvertBase64ToStream();
                //计算hash值，对于hash值的比较可以迅速判断文件是否重复
                string hash = HashHelper.ComputeSha256Hash(stream);
                //判断服务器中是否已经含有该文件，如果有则不再上传
                ServerFile? file = await _repository.FindAsync(hash);
                if (file != null) { 
                    images.Add((file.PreviewPhoto, file.FileUrl)!); 
                }
                //判断路径是否存在，如果不存在直接创建
                if (!Directory.Exists(route))
                {
                    Directory.CreateDirectory(route);
                }
                //上传到文件服务器中，返回可供直接访问的url
                var urls = await _storage.UploadPhoto(route + fileName, stream);
                //上传到数据库中
                var upFile = ServerFile.Create(fileName,hash,urls.higherUrl,stream.Length,UserId:userId,blogId:blogId,previewUrl:urls.previewUrl);
                await _repository.UploadAsync(upFile);
                images.Add((urls.previewUrl,urls.higherUrl)!);
            }
            return images!;
        }
    }
}
