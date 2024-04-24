using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class DateTimeExtension
    {
        public static string ConvertToChineseTime(this DateTime time)
        {
            TimeSpan timeDifference = DateTime.Now.Subtract(time);
            double minutes = timeDifference.TotalMinutes;
            string chineseTime = minutes switch
            {
                < 60 => (int)(minutes) + "分钟前",
                < 1440 => (int)(minutes / 60) + "小时前",
                < 43200 => (int)(minutes / (60 * 24)) + "天前",
                < 518400 => (int)(minutes / (60 * 24 * 30)) + "月前",
                _ => (int)(minutes / (60 * 24 * 30 * 12)) + "年前"
            };
            return chineseTime;
        }
    }
}
