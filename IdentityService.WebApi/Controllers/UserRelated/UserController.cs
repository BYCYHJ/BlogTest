using BlogDomainCommons;
using Identity.Domain;
using Identity.Domain.Entities;
using IdentityService.Infrastructure;
using IdentityService.WebApi.Protos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net;

namespace IdentityService.WebApi.Controllers.UserRelated
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IdentityDomainService idService;
        private readonly FileApi.FileApiClient _fileClient;

        public UserController(IHttpClientFactory httpClientFactory, IdentityDomainService idService, FileApi.FileApiClient fileClient)
        {
            _httpClientFactory = httpClientFactory;
            this.idService = idService;
            this._fileClient = fileClient;
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromBody] AvatarRequest request)
        {
            //查找用户
            User? targetUser = await idService.FindOneByIdAsync(request.userId);
            if (targetUser == null)
            {
                return BadRequest("找不到用户");
            }
            var response = await _fileClient.UploadPhotoAsync(new FileBase64Request
            {
                FileUrl = request.fileUrl,
                FileName = request.fileName,
                Base64Str = request.base64Str,
                UserId = request.userId,
            });
            targetUser.AvatarUrl = response.FileUrl;
            await idService.UpdateUserInfoAsync(targetUser);
            return Ok(response.FileUrl);

            //上传文件到文件服务器
            //var client = _httpClientFactory.CreateClient();
            //client.BaseAddress = new Uri("http://132.232.108.176:5214");
            //var response = await client.PostAsJsonAsync("/api/File/UploadBase64File", request);
            //if (!response.IsSuccessStatusCode)
            //{
            //    return BadRequest("文件上传失败");
            //}
            //var content = await response.Content.ReadAsStringAsync();
            //var UriJson = JsonConvert.DeserializeObject<AvatarResponse>(content);
            //if(UriJson!.code < 399)
            //{
            //    targetUser.AvatarUrl = UriJson.data;
            //    await idService.UpdateUserInfoAsync(targetUser);
            //}
            //else
            //{
            //    return BadRequest(UriJson.data);
            //}
        }

        /// <summary>
        /// 获取单个用户信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSingleUserInfo(string userId)
        {
            User? targetUser = await idService.FindOneByIdAsync(userId);
            //如果无目标返回错误
            if (targetUser == null) return BadRequest("无该用户");
            UserInfo user = new UserInfo(userId,avatarUrl:targetUser.AvatarUrl,userName:targetUser.UserName!);
            return Ok(user);
        }

        /// <summary>
        /// 获取多个用户信息
        /// </summary>
        /// <param name="userIds"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GetBulkUserInfo(IEnumerable<string> userIds)
        {
            var userInfos = idService
                .FindBulkUsers(userIds)
                .Select(info => new UserInfo(id: info.id, avatarUrl: info.avatarUrl, userName: info.name));
            return Ok(userInfos);
        }
    }
}
