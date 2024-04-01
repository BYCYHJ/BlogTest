namespace IdentityService.WebApi.Controllers.UserRelated
{
    public record AvatarRequest(
        string base64Str,
        string userId,
        string fileUrl,
        string fileName
        );

    public record AvatarResponse(
        int code,
        string message,
        string data
        );
}
