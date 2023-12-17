using BlogJWT;
using Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain
{
    public class IdentityService
    {
        private readonly IIdentityRepository _identityRepository;
        //private readonly ITokenService _tokenService;
        private readonly IOptions<JWTOptions> _jwtOpt;

        public IdentityService(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }

        /// <summary>
        /// 获取登录token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GetTokenAsync(User user)
        {
            var roles = await _identityRepository.GetUserRoleAsync(user);//用户角色
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()));
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role,role));
            }
            return "Aaa";
        }
    }
}
