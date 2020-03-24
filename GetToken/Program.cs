using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using qiyubrother;
namespace GetToken
{
    class Program
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);

        static void Main(string[] args)
        {
            qiyubrother.LogHelper.StartService();
            //var url = "http://39.100.15.130:8003/oauth2/login.html?client_id=1&response_type=code&username=wumaqun&password=ecde7f08840e41d35042aebc437bd3ff";
            var accounts = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "account.txt"));

            var ip = "192.168.10.168";
            var port = "8003";
            var pwd = TokenHelper.Md5("123456");
            var len = 10; // 最大并发度
            var timeout = 60; // 最长等待响应时间（秒）
            var taskList = new Task[len];
            for (var i = 0; i < len; i++)
            {
                int k = i;
                taskList[i] = new Task(() => TokenHelper.GetToken(ip, port, accounts[k], pwd, timeout));
            }
            for (var j = 0; j < len; j++)
            {
                taskList[j].Start();
            }
            Task.WaitAll(taskList);

            //TokenHelper.GetToken(ip, port, accounts[0], pwd);

            var s = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]Task finished.";
            qiyubrother.LogHelper.Trace(s);

            Console.ReadLine();
        }
    }
}
