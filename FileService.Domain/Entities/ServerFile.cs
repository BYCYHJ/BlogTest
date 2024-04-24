using BlogDomainCommons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Domain.Entities
{
    public class ServerFile : BaseEntity
    {
        public string FileName { get; private set; }//文件名称
        public string FileSha256Hash { get; private set; }//文件哈希值，负责快速比较文件是否重复
        public string FileUrl { get; private set; }//文件路径
        public string? PreviewPhoto {  get; private set; }//如果是图片，此为缩略图的预览路径
        public long FileSizeBytes { get; private set; } //文件大小
        public string? UserId { get; private set; }//上传者的id
        public string? BlogId {  get; private set; }//所属博客id

        //给efcore使用的构造方法
        private ServerFile() { }

        //创建一个文件对象
        public static ServerFile Create(string fileName, string hash, string fileUrl, long size,string? UserId = null,string? blogId = null,string? previewUrl=null)
        {
            ServerFile file = new ServerFile()
            {
                FileName = fileName,
                FileSha256Hash = hash,
                FileUrl = fileUrl,
                FileSizeBytes = size,
                UserId = UserId,
                BlogId = blogId,
                PreviewPhoto = previewUrl
            };
            return file;
        }
    }
}
