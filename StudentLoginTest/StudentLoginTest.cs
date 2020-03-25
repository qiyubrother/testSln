using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using qiyubrother;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace StudentLoginTest
{
    class StudentLoginTest
    {
        static HttpWebRequest[] HttpWebRequestArray = null;
        static void Main(string[] args)
        {
            qiyubrother.LogHelper.StartService();
            try
            {
                var cfg = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json"));
                JObject jo = (JObject)JsonConvert.DeserializeObject(cfg);

                var ip = CodeHelper.GetIP(jo["IP"].ToString());
                var port = jo["PORT"].ToString();
                var pwd = TokenHelper.Md5(jo["PASSWORD"].ToString());
                var len = Convert.ToInt32(jo["CONCURRENT"].ToString());   // 最大并发度
                var timeout = Convert.ToInt32(jo["TIMEOUT"].ToString());  // 最长等待响应时间（秒）
                var accountFile = jo["ACCOUNTFILE"].ToString();

                var accounts = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, accountFile));

                if (len > accounts.Length)
                {
                    len = accounts.Length;
                }

                var _ = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]Task begin. IP:{ip}, PORT:{port}, PASSWORD:{jo["PASSWORD"]}, CONCURRENT:{len}, TIMEOUT:{timeout}";
                LogHelper.Trace(_);
                Console.WriteLine(_);

                var taskList = new Task[len];
                for (var i = 0; i < len; i++)
                {
                    int k = i;
                    taskList[i] = new Task(() => TokenHelper.GetToken(ip, port, accounts[k], pwd, timeout, k));
                }
                taskList.ToList().ForEach(a => a.Start());

                //for (var j = 0; j < len; j++)
                //{
                //    taskList[j].Start();
                //}
                Task.WaitAll(taskList);

                var s = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]Task finished. Press enter to exit.";
                qiyubrother.LogHelper.Trace(s);
                Console.WriteLine(s);
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                LogHelper.Trace(ex.ToString());
                Console.WriteLine($"Exception. Press enter to exit.");
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }
        }
    }
}
