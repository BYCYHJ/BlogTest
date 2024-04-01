using FileService.Domain;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using System.Drawing;
using System.Dynamic;

namespace FileService.Infrastructure
{
    public class FileStorage : IFileStorage
    {
        public async Task<string> UploadPhoto(string filePath, Stream content)
        {
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
            return filePath;
        }

        //public string UploadPhoto(string filePath,Stream content)
        //{
        //    byte[] bytes = new byte[content.Length];
        //    content.Read(bytes, 0, bytes.Length);
        //    FileStream fs = new FileStream(filePath, FileMode.Create);
        //    BinaryWriter bw = new BinaryWriter(fs);
        //    bw.Write(bytes);
        //    bw.Close();
        //    fs.Close();
        //    return filePath;
        //}
    }
}
