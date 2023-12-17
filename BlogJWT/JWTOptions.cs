using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BlogJWT
{
    public class JWTOptions
    {
        public string Issuer {  get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public int ExpireSeconds { get; set; }//过期时间
    }
}
