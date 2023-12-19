using Identity.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace IdentityServiceWebApi.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IdentityDomainService idService;
        private readonly IIdentityRepository idRepository;

        public LoginController(IdentityDomainService idService,IIdentityRepository idRespository) {
            this.idService = idService;
            this.idRepository = idRespository;
        }

        [HttpPost]
        public async Task<ActionResult<string?>> LoginWithNameAndPwd(UserResponse loginInfo)
        {
            var result = await idService.CheckUserAndPasswordAsync(loginInfo.userName,loginInfo.password);
            if (!result.checkResult.Succeeded)
            {
                string msg = "用户名或密码不正确";
                return StatusCode((int)HttpStatusCode.BadRequest,msg);
            }else if (result.checkResult.IsLockedOut)
            {
                return StatusCode((int)HttpStatusCode.Locked,"此账号已被锁定");
            }
            return result.token;
        }
    }
}
