using ChatWorkServer.DTOs;
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
        public static string GetTimeSpan(DateTime CreatedDate) {
            TimeSpan span = DateTime.Now - CreatedDate;
            string spantime = "";
            if (span.TotalDays == 1)
            {
                spantime = string.Format("Yesterday");
            }
            else if (span.TotalDays > 1)
            {
                spantime = string.Format("{0} days", Math.Floor(span.TotalDays));
            }
            else if (span.TotalHours > 1)
            {
                spantime = string.Format("{0} hours ago", Math.Floor(span.TotalHours));
            }
            else if (span.TotalMinutes > 0)
            {
                spantime = string.Format("{0} minutes ago", Math.Floor(span.TotalMinutes));
            }
            else
            {
                spantime = string.Format("{0} seconds ago", Math.Floor(span.TotalSeconds));
            }
            return spantime;
        }
    }
}
