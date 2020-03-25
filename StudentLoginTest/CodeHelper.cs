
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class CodeHelper
    {
        /// <summary>
        /// 将域名转换为IP地址
        /// 如果传入的是IP地址，则直接返回
        /// 如果传入的是无效的域名，则反汇空字符串(string.Empty)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetIP(string url)
        {
            string ipAddress = url;
            if (!ipAddress.StartsWith("http"))
            {
                ipAddress = "http://" + ipAddress;
            }
            string p = @"(http|https)://(?<domain>[^(:|/]*)";
            var reg = new Regex(p, RegexOptions.IgnoreCase);
            Match m = reg.Match(ipAddress);
            try
            {
                string Result = m.Groups["domain"].Value;//提取域名地址 
                IPHostEntry host = Dns.GetHostByName(Result);//域名解析的IP地址
                IPAddress ip = host.AddressList[0];
                string rIP = ip.ToString();
                return rIP;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
