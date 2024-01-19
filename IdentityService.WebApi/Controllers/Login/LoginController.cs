using Identity.Domain.Entities;
using Identity.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace IdentityService.WebApi.Controllers.Login
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IdentityDomainService idService;
        private readonly IIdentityRepository idRepository;
        private readonly ISendMail mailSender;

        public LoginController(IdentityDomainService idService, IIdentityRepository idRespository, ISendMail mailSender)
        {
            this.idService = idService;
            this.idRepository = idRespository;
            this.mailSender = mailSender;
        }

        //根据用户名、密码登录
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string?>> LoginWithNameAndPwd([FromBody]UserResponse loginInfo)
        {
            var result = await idService.CheckUserAndPasswordAsync(loginInfo.userName, loginInfo.password);
            if (!result.checkResult.Succeeded)
            {
                string msg = "用户名或密码不正确";
                return StatusCode((int)HttpStatusCode.BadRequest, msg);
            }
            else if (result.checkResult.IsLockedOut)
            {
                return StatusCode((int)HttpStatusCode.Locked, "此账号已被锁定");
            }
            return StatusCode((int)HttpStatusCode.OK, result.token);
        }

        //获取验证码
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<String?>> GetPhoneTokenCode(string phone)
        {
            var user = await idRepository.FindOneByPhoneAsync(phone);
            if (user is null)
            {
                return StatusCode((int)HttpStatusCode.Unauthorized, "没有该账户");
            }
            var createResult = await idService.CreatePhoneTokenCodeAsync(phone);
            if (!createResult.Item1.Succeeded)
            {
                return StatusCode((int)HttpStatusCode.BadGateway, "未成功创建验证码请重新发送");
            }
            //开始发送验证码
            await mailSender.SendSmsMail(phone, new string[] { "code", createResult.code! });
            return StatusCode((int)HttpStatusCode.OK, createResult.code);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<string?>> LoginWithPhone(string phone,string code)
        {
            var result = await idService.CheckPhoneAndTokenAsync(phone, code);
            if (result.Succeeded)
            {
                User user = (await idRepository.FindOneByPhoneAsync(phone))!;
                var token = await idService.GetTokenAsync(user);
                return StatusCode((int)HttpStatusCode.OK, token);
            }
            return StatusCode((int)HttpStatusCode.BadGateway, "验证码不正确");
        }

        //获取当前用户的信息
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<object>> GetUserInfo()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier)!;//用户id
            var user = await idRepository.FindOneUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            else return new { userName = user.UserName, phoneNumber = user.PhoneNumber };
        }

        [Authorize]
        [HttpPatch]
        public async Task<ActionResult> ChangePassword([FromBody]UpdatePwdResponse pwdResponse)
        {
            var userId = Guid.Parse(this.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await idRepository.ChangePwdAsync(userId, pwdResponse.currentPwd, pwdResponse.newPwd);
            if (result.Succeeded)
            {
                return Ok();
            }
            //返回错误集
            return BadRequest(result.Errors.SumErrors());
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> CreateWorld()
        {
            string username = "AnAn";
            string password = "15550239";
            var result = await idRepository.CreateUserAsync(username, password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.SumErrors());
            }
            var role1 = "admin";
            var role2 = "normalUser";
            try
            {
                await idRepository.CreateRoleAsync(role1);
                await idRepository.CreateRoleAsync(role2);
                var user = await idRepository.FindOneUserByNameAsync(username);
                await idRepository.AddRoleToUserAsync(user, role1);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
