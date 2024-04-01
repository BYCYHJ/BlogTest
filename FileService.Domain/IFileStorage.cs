using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain
{
    public interface IFileStorage
    {
        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="key">存放路径</param>
        /// <param name="content">文件流</param>
        /// <returns></returns>
        //string UploadFileAsync(string route,string base64Content);
        //string UploadFileAsync(string route, IFormFile file);
        Task<string> UploadPhoto(string filePath,Stream content);
    }
}
