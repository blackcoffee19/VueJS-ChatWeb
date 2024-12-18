using System.Security.Cryptography;
using System.Text;

namespace ChatWorkServer.Common
{
    public class TUtility
    {
        public static string GetMD5(string str)
        {
            return GetSHA(str, MD5.Create());
        }
        private static string GetSHA(string str, HashAlgorithm hash)
        {
            try
            {
                string[] value = (from x in hash.ComputeHash(Encoding.UTF8.GetBytes(str))
                                  select x.ToString("X2")).ToArray<string>();
                return string.Join("", value).ToUpper();
            }
            catch
            {
            }
            return "";
        }
    }
}
