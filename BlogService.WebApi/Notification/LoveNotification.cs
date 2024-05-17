using BlogService.Domain.Entities;
using MediatR;

namespace BlogService.WebApi.Notification
{
    public record LoveNotification(string userId,string objectId,HeartType type) : INotification;
}
