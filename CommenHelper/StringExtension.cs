using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtension
    {
        /// <summary>
        /// base64字符串去除头部信息后转为Stream
        /// </summary>
        /// <param name="base64Str"></param>
        /// <returns></returns>
        public static Stream ConvertBase64ToStream(this string base64Str)
        {
            string tempBase64Str = base64Str.Replace("data:image/png;base64,", "")
                .Replace("data:image/jgp;base64,", "")
                .Replace("data:image/jpg;base64,", "")
                .Replace("data:image/jpeg;base64,", "");
            byte[] bytes = Convert.FromBase64String(tempBase64Str);
            return new MemoryStream(bytes);
        }
    }
}
