using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BlogHashHelper
{
    public static class HashHelper
    {
        private static string ToHashString(byte[] data)
        {
            StringBuilder builder = new StringBuilder();
            foreach (byte b in data)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// sha256哈希
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ComputeSha256Hash(Stream stream)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(stream);
                stream.Seek(0, SeekOrigin.Begin);//恢复起始点
                return ToHashString(hash);
            }
        }

        public static string ComputeSha256Hash(string data)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
                return ToHashString(hash);
            }
        }

        public static string ComputeMD5Hash(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                stream.Seek(0, SeekOrigin.Begin);//恢复起始点
                return ToHashString(hash);
            }
        }

        public static string ComputeMD5Hash(string data)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(data));
                return ToHashString(hash);
            }
        }
    }
}
