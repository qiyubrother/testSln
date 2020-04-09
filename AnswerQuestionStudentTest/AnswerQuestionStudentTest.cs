using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using qiyubrother;
using RabbitMQ.Client;

namespace AnswerQuestionStudentTest
{
    class AnswerQuestionStudentTest
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern void OutputDebugString(string message);
        static void Main(string[] args)
        {
            var cfg = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json"));
            JObject jo = (JObject)JsonConvert.DeserializeObject(cfg);

            LogHelper.StartService();
            var accountIds = RemoveEmptyLines(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "accountId.txt")));
            var resourceIds = ParseResourceLines(RemoveEmptyLines(File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "resourceId.txt"))));
            var mqIp = GetIP(jo["MQIP"].ToString());
            var mqPort = jo["MQPORT"].ToString();
            var mqUserName = jo["MQUSERNAME"].ToString();
            var mqPassword = jo["MQPASSWORD"].ToString();

            var connection = MQHelper.CreateMqConnection(mqIp, mqPort, mqUserName, mqPassword);

            IModel channel = connection.CreateModel();

            // 生成随机队列名称
            var queueName = channel.QueueDeclare().QueueName;

            var len = accountIds.Length * resourceIds.Length; // 最大并发度
            var lstTask = new Task[len];
            var index = 0;
            for (var m = 0; m < accountIds.Length; m++)
            {
                var aIndex = m;
                for(var n = 0; n < resourceIds.Length; n++)
                {
                    int k = index;
                    int rIndex = n;
                    lstTask[k] = new Task(() =>
                    {
                        var j = new JObject();
                        var jItem = new JObject();
                        jItem["Publish_Details_ID"] = resourceIds[rIndex].ResourceDetailId;
                        jItem["StudentAnswer"] = "B";
                        jItem["RightOrWrong"] = 0;
                        jItem["Sort"] = 1;
                        var jArray = new JArray();
                        jArray.Add(jItem);
                        j["AnswerList"] = jArray;
                        j["ErrorNo"] = "1";
                        j["Time_Table_ID"] = "7a6177f3980848ef81f1a6355f83cb4c"; // "a9731c294ac7405dab551e69e93d6941";
                        j["Resource_ID"] = resourceIds[rIndex].ResourceId;
                        j["Course_ID"] = "33de65df44004ec49059a929a62d1979";
                        j["Account_ID"] = accountIds[aIndex];
                        j["Answer_Time"] = "2020-4-8 16:41:07";
                        j["type"] = "1";
                        j["accessToken"] = "921a71d35fd7ea9aaa8c75185907d299";

                        var jsonPostData = j.ToString();
                        //LogHelper.OutputDebugString($"MQ::{jsonPostData}");
                        string routingKey = "tea.question.courseId." + j["Time_Table_ID"].ToString();
                        var serverExchangeName = "server-topic-exchange";
                        //MQHelper.sentMsgToMQ(routingKey, channel, queueName, jsonPostData, serverExchangeName);
                        MQHelper.sentMsgToMQ(routingKey, jsonPostData, serverExchangeName);
                    });
                    index++;
                }
            }
            var ss = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]Start.";
            LogHelper.Trace(ss);
            Console.WriteLine(ss);
            for (var i = 0; i < len; i++)
            {
                lstTask[i].Start();
            }
            Task.WaitAll(lstTask);

            var s = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]Task finished. Total send mq={len}";
            LogHelper.Trace(s);
            Console.WriteLine(s);
            Console.ReadLine();
        }

        public static string[] RemoveEmptyLines(string[] lines)
        {
            var lst = new List<string>(lines);
            for(var i = lines.Length - 1; i >= 0; i--)
            {
                if (lst[i] == string.Empty)
                {
                    lst.RemoveAt(i);
                }
            }

            return lst.ToArray();
        }

        public static ResourceData[] ParseResourceLines(string[] lines)
        {
            var lst = new List<ResourceData>();

            foreach(var line in lines)
            {
                var items = line.Split(' ');
                lst.Add(new ResourceData { ResourceId = items[0], ResourceDetailId = items[1] });
            }

            return lst.ToArray();
        }

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

    class ResourceData
    {
        public string ResourceId { get; set; }
        public string ResourceDetailId { get; set; }
    }
}
