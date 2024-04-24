using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommenHelper
{
    public static class HtmlHelper
    {
        /// <summary>
        /// 将html文本去除标签，保留纯文本
        /// </summary>
        /// <param name="htmlStr"></param>
        /// <returns></returns>
        public static string RemoveTags(string htmlStr)
        {
            var regex = new Regex("</?[^>]*>");
            MatchCollection matches = regex.Matches(htmlStr);
            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    htmlStr = htmlStr.Replace(match.Value, "");
                }
            }
            return htmlStr;
        }
    }
}
