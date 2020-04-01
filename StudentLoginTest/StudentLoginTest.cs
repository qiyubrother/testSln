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
using System.Threading;
using System.Threading.Tasks;
using qiyubrother;
using System.Collections;

namespace StudentLoginTest
{
    class StudentLoginTest
    {
        static HttpWebRequest[] HttpWebRequestArray = null;
        static void Main(string[] args)
        {
            var cfg = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json"));
            JObject jo = (JObject)JsonConvert.DeserializeObject(cfg);

            LogHelper.StartService();
            
            var classId = "AAAA-BBBB-CCCC-DDDD";
            var timeTableId = "2019-2020-2021";
            var accountId = "liuzhenhua";
            var accessToken = "ACC-ESS-TOKEN-2020";
            var T_T_T_BH = "DEMO-TTT-BH";
            var url = jo["URL"].ToString();
            var numberOfRuns = Convert.ToInt32(jo["NUMBEROFRUNS"].ToString());   // 最大运行次数
            var mqIp = jo["MQIP"].ToString();
            var mqPort = jo["MQPORT"].ToString();
            var mqUserName = jo["MQUSERNAME"].ToString();
            var mqPassword = jo["MQPASSWORD"].ToString();
            Task[] mqKaoQinTask = new Task[numberOfRuns];
            MQHelper.CreateMqConnection(mqIp, mqPort, mqUserName, mqPassword);

            bool isExitTask = false;
            for (var i = 0; i < numberOfRuns; i++)
            {
                var k = i;
                mqKaoQinTask[i] = new Task(() => {

                    // 获取上课信息
                    string[] param = new string[] { accountId, "21", accessToken };

                    LogHelper.Trace($"param::accountId:{param[0]},applicationType:{param[1]},accessToken:{param[2]}");
                    var s = WebServiceHelper.CallMethod(url, "getCourseInfoWhenStudentLoginInfo", param);
                    LogHelper.Trace(s);
                    // 考勤
                    var jObj = new JObject();
                    jObj.Add(new JProperty("studentAccountId", accountId));
                    jObj.Add(new JProperty("timeTableId", timeTableId));
                    jObj.Add(new JProperty("wifiName", "Wifi-liuzhenhua"));
                    jObj.Add(new JProperty("checkWorkStatus", "2"));
                    jObj.Add(new JProperty("checkWorkTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    jObj.Add(new JProperty("classId", classId));
                    jObj.Add(new JProperty("tocken", accessToken));
                    var jsonPostData = jObj.ToString();
                    LogHelper.OutputDebugString($"MQ::{jsonPostData}");
                    MQHelper.sentMsgToMQqueue("checkwork", jsonPostData);
                });
            };
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]开始并发测试MQ（考勤）...");
            mqKaoQinTask.ToList().ForEach(a => a.Start());
            #region 考勤统计
            Task.Run(() =>
            {
                while (!isExitTask)
                {
                    string method2 = "getCheckWorkStatistics";
                    string[] param2 = new string[] { timeTableId, T_T_T_BH, accessToken };
                    LogHelper.Trace("param::timeTableId:{0},T_T_T_BH:{1},token:{2}", param2[0], param2[1], param2[2]);
                    var s2 = WebServiceHelper.CallMethod(url, method2, param2);
                    LogHelper.Trace(s2);
                    LogHelper.Trace("Sleep 5000 ms.");
                    Thread.Sleep(5000);
                }
            });
            #endregion
            Task.WaitAll(mqKaoQinTask, 1000 * 60 * 5); // 最长等待5分钟
            isExitTask = true;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]完成...");
            LogHelper.Stop();
        }
    }
}
