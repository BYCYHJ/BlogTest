using BlogService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogService.Domain.Dtos
{
    public class BlogWithUserInfo
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string? AvatarUrl { get; set; }
        public string PreviewPhoto {  get; set; }
        public IEnumerable<TagClass>? Tags { get; set; }

        public BlogWithUserInfo(string id, string title, string content, string userId, string userName, string? avatarUrl,string previewPhoto,IEnumerable<TagClass> tags)
        {
            Id = id;
            Title = title;
            Content = content;
            UserId = userId;
            UserName = userName;
            AvatarUrl = avatarUrl;
            PreviewPhoto = previewPhoto;
            Tags = tags;
        }
    }
}
