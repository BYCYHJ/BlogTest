using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BlogJWT
{
    public interface ITokenService
    {
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="jwtOpt"></param>
        /// <returns></returns>
        string BuildToken(List<Claim> claims,JWTOptions jwtOpt);
    }
}
