using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using qiyubrother;

namespace SocketHttp
{
    class SocketHttpMain
    {
        static void Main(string[] args)
        {
            var cfg = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json"));
            JObject jo = (JObject)JsonConvert.DeserializeObject(cfg);

            var ip = CodeHelper.GetIP(jo["IP"].ToString());
            var port = jo["PORT"].ToString();
            var pwd = TokenHelper.Md5(jo["PASSWORD"].ToString());
            var numberOfRuns = Convert.ToInt32(jo["NUMBEROFRUNS"].ToString());   // 最大运行次数
            var accountFile = jo["ACCOUNTFILE"].ToString();
            var accountDetailFile = jo["ACCOUNTDETAILFILE"].ToString();
            var timeTableId = jo["TIMETABLEID"].ToString();
            var classId = jo["CLASSID"].ToString();
            var accounts = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, accountFile));
            var teachingPlanId = jo["TEACHINGPLANID"].ToString();
            var resourceId = jo["RESOURCEID"].ToString();
            var mqIp = jo["MQIP"].ToString();
            var mqPort = jo["MQPORT"].ToString();
            var mqUserName = jo["MQUSERNAME"].ToString();
            var mqPassword = jo["MQPASSWORD"].ToString();
            var serverExchangeName = jo["SERVEREXCHANGENAME"].ToString();
            //var maxDegreeOfParallelism = Convert.ToInt32(jo["CONCURRENT"].ToString());
            MQHelper.CreateMqConnection(mqIp, mqPort, mqUserName, mqPassword);
            ConcurrentDictionary<string, string> cd = new ConcurrentDictionary<string, string>();
            BlockingCollection<MData> bc = new BlockingCollection<MData>();

            // 装置 AccountId 与 Account 的关系
            var accountDetails = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, accountDetailFile));
            foreach(var item in accountDetails)
            {
                var arr = item.Split('\t');
                cd.TryAdd(arr[1], arr[0]);
            }

            var r = new Random((int)DateTime.Now.ToFileTimeUtc());
            var accountIndexArray = new int[numberOfRuns];
            for(var i = 0; i < numberOfRuns; i++)
            {
                accountIndexArray[i] = r.Next(0, accounts.Length - 1);
            }

            var taskList = new Task[numberOfRuns];
            var startDateTime = $"[{ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]";
            for (var i = 0; i < numberOfRuns; i++)
            {
                int k = i;
                taskList[i] = new Task(() =>
                {
                    var account = string.Empty;
                    account = accounts[accountIndexArray[k]];
                    var url = $"http://{ip}:{port}/oauth2/login.html?client_id=1&response_type=code&username={account}&password={pwd}";
                    SocketHttpHelper.SocketHttpPost(ip, port, url, out string status, out string data);
                    LogHelper.OutputDebugString($"[{k}]{status}, {data}");

                    if (status == SocketHttpHelper.HttpState.S200)
                    {
                        JObject __jo = JsonConvert.DeserializeObject<JObject>(data);
                        try
                        {
                            var t = __jo["code"].ToString() + __jo["state"].ToString();

                            var code = __jo["code"].ToString(); // 取得CODE
                            var state = __jo["state"].ToString();
                            string client_secret = "111";
                            string baseUrl = "http://" + ip + ":" + port;
                            string tokenUrl = baseUrl + "/oauth2/token";
                            string redirect_uri = baseUrl + "/oauth2/getToken.do";
                            var refreshToken = string.Empty;

                            var body = $"grant_type=authorization_code&client_id=1&client_secret={client_secret}&refresh_token={refreshToken}&username={account}&redirect_uri={redirect_uri.Replace("&", "%26%")}&code={code}&state={state}";
                            SocketHttpHelper.SocketHttpPost(ip, port, tokenUrl, body, out status, out data);
                            LogHelper.OutputDebugString($"[{k}]{status}, {data}");

                            // 保存有效数据，用于之后的并发测试
                            if (status == SocketHttpHelper.HttpState.S200)
                            {
                                var joAccessToken = JsonConvert.DeserializeObject<JObject>(data);
                                var accessToken = joAccessToken["access_token"].ToString();
                                refreshToken = joAccessToken["refresh_token"].ToString();
                                bc.Add(new MData
                                {
                                    AccessToken = accessToken,
                                    RefreshToken = refreshToken,
                                    Code = code,
                                    State = state,
                                    Account = account,
                                    AccountId = cd[account],
                                    ClassId = classId,
                                    TimeTableId = timeTableId
                                });
                            }
                        }
                        catch { }
                    }
                });
            }
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]开始获取Code、AccessToken...");
            taskList.ToList().ForEach(a => a.Start());
            //Parallel.For(0, taskList.Length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, i => { taskList[i].Start(); });

            Task.WaitAll(taskList, 1000 * 60 * 10); // 最长等待10分钟
#if false
#region 并发测试MQ(考勤)
            var bcTaskList = bc.ToList();
            var mqKaoQinTask = new Task[bcTaskList.Count];
            for (var i = 0; i < bcTaskList.Count; i++)
            {
                var k = i;
                mqKaoQinTask[i] = new Task(() => {
                    var a = bcTaskList[k];
                    var jObj = new JObject();
                    jObj.Add(new JProperty("studentAccountId", a.AccountId));
                    jObj.Add(new JProperty("timeTableId", a.TimeTableId));
                    jObj.Add(new JProperty("wifiName", "Wifi-liuzhenhua"));
                    jObj.Add(new JProperty("checkWorkStatus", "2"));
                    jObj.Add(new JProperty("checkWorkTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    jObj.Add(new JProperty("classId", a.ClassId));
                    jObj.Add(new JProperty("tocken", a.AccessToken));
                    var jsonPostData = jObj.ToString();
                    LogHelper.OutputDebugString($"MQ::{jsonPostData}");
                    MQHelper.sentMsgToMQqueue("checkwork", jsonPostData);
                });
            };
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]开始并发测试MQ（考勤）...");
            mqKaoQinTask.ToList().ForEach(a => a.Start());
            //Parallel.For(0, mqKaoQinTask.Length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, i => { mqKaoQinTask[i].Start(); });

            Task.WaitAll(mqKaoQinTask, 1000 * 60 * 5); // 最长等待5分钟
#endregion

#region 并发测试MQ(举手发言)
            var mqJuShouFaYanTask = new Task[bcTaskList.Count];
            for (var i = 0; i < bcTaskList.Count; i++)
            {
                var k = i;
                mqJuShouFaYanTask[i] = new Task(() => {
                    var a = bcTaskList[k];
                    var jObj = new JObject();

                    jObj.Add(new JProperty("Course_ID", teachingPlanId));
                    jObj.Add(new JProperty("Time_Table_ID", a.TimeTableId));
                    jObj.Add(new JProperty("Account_ID", a.AccountId));
                    jObj.Add(new JProperty("accessToken", a.AccessToken));

                    var jsonPostData = jObj.ToString();
                    LogHelper.OutputDebugString($"MQ::{jsonPostData}");
                    MQHelper.sentMsgToMQqueue("student_speak", jObj.ToString());
                });
            };
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]开始并发测试MQ（举手发言）...");
            mqJuShouFaYanTask.ToList().ForEach(a => a.Start());
            //Parallel.For(0, mqJuShouFaYanTask.Length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, i => { mqJuShouFaYanTask[i].Start(); });

            Task.WaitAll(mqJuShouFaYanTask, 1000 * 60 * 5); // 最长等待5分钟
#endregion

#region 并发测试MQ（回答问题）
            var mqHuiDaWenTiTask = new Task[bcTaskList.Count];
            for (var i = 0; i < bcTaskList.Count; i++)
            {
                var k = i;
                mqHuiDaWenTiTask[i] = new Task(() => {
                    var a = bcTaskList[k];
                    var jObj = new JObject();

                    jObj.Add(new JProperty("Time_Table_ID", a.TimeTableId));
                    jObj.Add(new JProperty("Resource_ID", resourceId));
                    jObj.Add(new JProperty("Course_ID", teachingPlanId));
                    jObj.Add(new JProperty("Account_ID", a.AccountId));
                    jObj.Add(new JProperty("Answer_Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                    jObj.Add(new JProperty("type", "1"));
                    jObj.Add(new JProperty("accessToken", a.AccessToken));

                    var jsonPostData = jObj.ToString();
                    LogHelper.OutputDebugString($"MQ::{jsonPostData}");
                    string routingKey = "tea.question.courseId." + a.TimeTableId;
                    MQHelper.sentMsgToMQ(routingKey, jsonPostData, serverExchangeName);
                });
            };
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]开始并发测试MQ（回答问题）...");
            mqHuiDaWenTiTask.ToList().ForEach(a => a.Start());
            //Parallel.For(0, mqHuiDaWenTiTask.Length, new ParallelOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism }, i => { mqHuiDaWenTiTask[i].Start(); });

            Task.WaitAll(mqHuiDaWenTiTask, 1000 * 60 * 5); // 最长等待5分钟
#endregion
#endif
            var s = $"Run from {startDateTime} to [{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}], 总运行次数:{numberOfRuns}. All of task finished. Press enter to exit.";
            LogHelper.OutputDebugString(s);
            Console.WriteLine(s);
            Console.ReadLine();
            Environment.Exit(0);
        }
    }

    public class MData
    {
        public string Account { get; set; }
        public string AccountId { get; set; }
        public string Code { get; set; }
        public string State { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClassId { get; set; }
        public string TimeTableId { get; set; }
    }
}
