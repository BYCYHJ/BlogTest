using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogJWT
{
    public class TokenService : ITokenService
    {
        public string BuildToken(List<Claim> claims, JWTOptions jwtOpt)
        {
            TimeSpan expireSpan = TimeSpan.FromSeconds(jwtOpt.ExpireSeconds);//过期时间
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOpt.Key));
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer: jwtOpt.Issuer, 
                audience:jwtOpt.Audience,
                claims:claims,
                expires:DateTime.Now.Add(expireSpan),
                signingCredentials:credentials
                ) ;
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
