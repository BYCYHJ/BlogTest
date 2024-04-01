using BlogDomainCommons;
using Identity.Domain;
using Identity.Domain.Entities;
using IdentityService.Infrastructure;
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

        public UserController(IHttpClientFactory httpClientFactory, IdentityDomainService idService)
        {
            _httpClientFactory = httpClientFactory;
            this.idService = idService;
        }

        /// <summary>
        /// 更新用户头像
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [UnitofWork([typeof(IdServiceDbContext)])]
        public async Task<IActionResult> UploadAvatar([FromBody] AvatarRequest request)
        {
            //查找用户
            User? targetUser = await idService.FindOneByIdAsync(request.userId);
            if (targetUser == null)
            {
                return BadRequest("找不到用户");
            }
            //上传文件到文件服务器
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://132.232.108.176:5214");
            var response = await client.PostAsJsonAsync("/api/File/UploadBase64File", request);
            if (!response.IsSuccessStatusCode)
            {
                return BadRequest("文件上传失败");
            }
            var content = await response.Content.ReadAsStringAsync();
            var UriJson = JsonConvert.DeserializeObject<AvatarResponse>(content);
            if(UriJson!.code < 399)
            {
                targetUser.AvatarUrl = UriJson.data;
                await idService.UpdateUserInfoAsync(targetUser);
            }
            else
            {
                return BadRequest(UriJson.data);
            }
            return Ok(UriJson!.data);
        }

    }
}
