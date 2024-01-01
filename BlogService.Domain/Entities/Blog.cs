﻿using BlogDomainCommons;

namespace BlogService.Domain.Entities
{
    public class Blog : BaseEntity
    {
        public string Title { get; private set; }//博客标题
        public string Content { get; private set; }//文章内容
        public List<TagClass> Tags { get; private set; } = new List<TagClass>();//文章标签
        public string UserId { get; init; }//作者
        public int StartCount { get; private set; } = 0;//点赞数

        public Blog(string title, string content, string userId,List<TagClass>? tags = null)
        {
            Title = title;
            Content = content;
            UserId = userId;
            if(tags is not null && tags.Any())
            {
                this.Tags.AddRange(tags);
            }
        }

        //修改博客
        public void ChangeBlog(Blog blog)
        {
            this.ChangeBlog(blog.Title,blog.Content,blog.Tags);
        }

        public void ChangeBlog(string title,string content,List<TagClass>? tags)
        {
            this.Title = title;
            this.Content = content;
            if(tags is not null && tags.Any())
            {
                this.Tags = tags;
            }
            else
            {
                this.Tags = new List<TagClass>();
            }
        }

        //增加点赞数
        public void AddStar()
        {
            StartCount++;
        }

        //取消点赞
        public void SubStar()
        {
            StartCount--;
        }

        public void AddStars()
        {
            this.StarCount++;
        }

        public void RemoveStars()
        {
            this.RemoveStars();
        }
    }
}