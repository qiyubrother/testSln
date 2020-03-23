using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using qiyubrother;
using System.IO;
namespace StudentSubmitQuestionA
{
    class Program
    {
        static string ip = "";
        static string port = "";
        static string pwd = string.Empty;
        static string timeTableId = "";    // 课表ID
        static string courseId = "";       // 课程ID
        static string resourceId = ""; // 问题资源ID
        static string preclass_server_url = "http://192.168.10.168:8081/ws/IEasySecServer?wsdl";
        static List<string> answserList = new List<string>();
        static string studentFileName = string.Empty;
        static void Main(string[] args)
        {
            LogHelper.StartService();

            var s = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "config.json"));
            JObject jo = (JObject)JsonConvert.DeserializeObject(s);
            ip = jo["LOGIN_IP"].ToString();
            port = jo["LOGIN_PORT"].ToString();
            preclass_server_url = jo["PRECLASS_SERVER_URL"].ToString();
            timeTableId = jo["TIME_TABLE_ID"].ToString();
            courseId = jo["COURSE_ID"].ToString();
            resourceId = jo["RESOURCE_ID"].ToString();
            pwd = TokenHelper.Md5(jo["PASSWORD"].ToString());
            studentFileName = Path.Combine(Environment.CurrentDirectory, jo["STUDENT_FILE_NAME"].ToString());
            var studentAccountList = StudentHelper.GetStudentList(studentFileName);
            // 读取答案列表
            answserList = AnswerHelper.GetAnswerList(Path.Combine(Environment.CurrentDirectory, "answers"));

            for (var i = 0; i < studentAccountList.Count; i++)
            {
                try
                {
                    Task.Run(()=> Work(studentAccountList[i]));
                    //Work(studentAccountList[i]);
                    break;
                }
                catch(Exception ex)
                {
                    LogHelper.Trace(ex.Message);
                }
            }
        }

        public static void Work(string studentName)
        {
            // get access token
            var accessToken = TokenHelper.GetToken(ip, port, studentName, pwd);
            var answer = AnswerHelper.RandomOneOfAnswer(answserList); // ONE OF ANSWER LIST
            // 回答问题
            AnswerQuestionStudent(studentName, answer, accessToken);
        }

        public static string AnswerQuestionStudent(string studentName, string answer, string accessToken)
        {
            string method = "answerQuestionStudent";

            JObject obj = JObject.Parse(answer);
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Time_Table_ID", timeTableId));
            jObj.Add(new JProperty("Resource_ID", resourceId));
            jObj.Add(new JProperty("Course_ID", courseId));
            jObj.Add(new JProperty("Account_Name", studentName));
            jObj.Add(new JProperty("accessToken", accessToken));
            string[] param = new string[] { jObj.ToString() };

            try
            {
                string strRtn = WebServiceHelper.CallMethod(preclass_server_url, method, param);
                LogHelper.Trace("【{0}】 return:{1}", method, strRtn);
                if (string.IsNullOrEmpty(strRtn))
                {
                    LogHelper.Trace("WSanswerQuestionStudent:请求结果为空!");
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(strRtn);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return strRtn;
                }
                else
                {
                    LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace($"{method}, {ex.GetType()}, {ex.Message}, {ex.Source}, {ex.StackTrace}");
                return "";
            }
            finally
            {
                ;
            }
        }
    }
}
