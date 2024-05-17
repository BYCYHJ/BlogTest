using Grpc.Core;
using Identity.Domain;
using Identity.Domain.Entities;
using IdentityService.GrpcApi;

namespace IdentityService.GrpcApi.Services
{
    public class UserProtoService : UserApi.UserApiBase
    {
        private readonly IdentityDomainService idService;
        public UserProtoService(IdentityDomainService idService)
        {
            this.idService = idService;
        }

        public override Task<BulkUserInfo> GetBulkUserInfo(BulkUserId request, ServerCallContext context)
        {
            try
            {
                var userInfos = idService
                    .FindBulkUsers(request.Ids)
                    .Select(info => new UserInfo
                    {
                        Id = info.id,
                        UserName = info.name,
                        AvatarUrl = info.avatarUrl == null ? "" : info.avatarUrl,
                    }).ToList();
                BulkUserInfo result = new BulkUserInfo();
                result.UserInfos.AddRange(userInfos);
                return Task.FromResult(result);
            }catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal,ex.Message));
            }
        }

        public override async Task<UserInfo> GetSingleUserInfo(UserId request, ServerCallContext context)
        {
            try
            {
                User? targetUser = await idService.FindOneByIdAsync(request.Id);
                //如果无目标返回错误
                if (targetUser == null) throw new RpcException(new Status(StatusCode.DataLoss, $"无法找到id为{request.Id}的用户"));
                UserInfo info = new UserInfo
                {
                    UserName = targetUser.UserName,
                    AvatarUrl = targetUser.AvatarUrl,
                    Id = request.Id
                };
                return info;
            }catch(Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal,ex.Message));
            }
        }
    }
}
