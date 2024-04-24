using FileService.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System.Drawing;
using System.Dynamic;

namespace FileService.Infrastructure
{
    public class FileStorage : IFileStorage
    {
        private readonly IConfiguration configuration;//用于获得appsetting中配置信息
        public FileStorage(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<(string previewUrl, string higherUrl)> UploadPhoto(string filePath, Stream content)
        {
            string serverUrl = configuration.GetSection("FileServer").Value!;
            //保存图片
            var image = await Image.LoadAsync(content);
            string suffix = Path.GetExtension(filePath).ToLower();
            IImageEncoder encoder = suffix switch
            {
                ".png" => new PngEncoder(),
                ".jpg" => new JpegEncoder(),
                ".jpeg" => new JpegEncoder(),
                ".bmp" => new BmpEncoder(),
                _ => new JpegEncoder()
            };
            await image.SaveAsync(filePath, encoder);
            //返回图片地址
            string fileName = Path.GetFileName(filePath);
            string higherUrl = "http://" + serverUrl + "/" + fileName;
            //缩略图
            string fileRoute = Path.GetDirectoryName(filePath)!;
            string previewRoute = fileRoute + "/preview_" + fileName;
            image.Mutate(i => i.Resize(100, 0));
            await image.SaveAsync(previewRoute, encoder);
            string previewUrl = "http://" + serverUrl + "/" + "preview_" + fileName;
            return (previewUrl, higherUrl);
        }
    }
}
