﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogJWT
{
    public class TokenCommon
    {
        /// <summary>
        /// 返回payload的负载信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Claim> GetPayloadInfo(string token)
        {
            string tokenStr = token.Replace("Bearer ","");
            var handler = new JwtSecurityTokenHandler();
            var payload = handler.ReadJwtToken(tokenStr).Payload;
            var claims = payload.Claims;
            return claims;
        }
    }
}
