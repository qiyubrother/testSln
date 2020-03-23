using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class TokenHelper
    {
        public static string GetToken(string ip, string port, string account, string pwd)
        {
            GetCode(ip, port, account, pwd, out string code, out string state);
            return code == string.Empty ? string.Empty : GetToken(ip, port, account, code, state, "authorization_code");
        }

        private static void GetCode(string ip, string port, string account, string pwd, out string code, out string state)
        {
            var s = string.Empty;
            try
            {
                var url = $"http://{ip}:{port}/oauth2/login.html?client_id=1&response_type=code&username={account}&password={pwd}";
                LogHelper.Trace($"GetCode...");
                s = HttpPost(url, string.Empty);
                JObject jo = (JObject)JsonConvert.DeserializeObject(s);
                LogHelper.Trace($"[RTN]String={s}");
                if (jo["msgCode"].ToString() == "200")
                {
                    code = jo["code"].ToString();
                    state = jo["state"].ToString();
                }
                else
                {
                    code = string.Empty;
                    state = string.Empty;
                }
            }
            catch(NullReferenceException nfe)
            {
                LogHelper.Trace($"[GetToken][NullReferenceException]:{nfe.InnerException}");
                code = string.Empty;
                state = string.Empty;
            }
            catch(Exception ex)
            {
                LogHelper.Trace($"[GetCode][{ex.GetType()}]:{ex.Message}");
                code = string.Empty;
                state = string.Empty;
            }
        }

        private static string GetToken(string ip, string port, string account, string code, string state, string grantType)
        {
            string client_secret = "111";
            string baseUrl = "http://" + ip + ":" + port;
            String tokenUrl = baseUrl + "/oauth2/token";
            String redirect_uri = baseUrl + "/oauth2/getToken.do";

            using (HttpClient httpClient = new HttpClient())
            {
                int timout_sec = 30; // 最长等待时间（秒）
                httpClient.Timeout = TimeSpan.FromSeconds(timout_sec);
                var refreshToken = string.Empty;
                var param = new Dictionary<string, string>()
                {
                    {"client_id", "1"},
                    {"client_secret", client_secret},
                    {"refresh_token", refreshToken},
                    {"username", account},
                    {"redirect_uri", redirect_uri},
                    {"code", code},
                    {"state", state},
                    {"grant_type",grantType}
                };
                var httpContent = new FormUrlEncodedContent(param);
                LogHelper.Trace($"GetToken...");
                try
                {
                    var response = httpClient.PostAsync(tokenUrl, httpContent).Result;
                    response.EnsureSuccessStatusCode();
                    string responseValue = response.Content.ReadAsStringAsync().Result;

                    JObject jot = (JObject)JsonConvert.DeserializeObject(responseValue);
                    string err_code = jot["err_code"].ToString();
                    LogHelper.Trace($"[RTN]err_code={err_code}");
                    if (err_code == "200")
                    {
                        LogHelper.Trace($"access_token:{jot["access_token"]}...");
                        return jot["access_token"].ToString();
                    }
                    else
                    {
                        LogHelper.Trace($"[getToken]err_code:{err_code}");
                        return string.Empty;
                    }
                }
                catch (AggregateException ae)
                {
                    LogHelper.Trace($"[GetToken][AggregateException][响应超时导致的请求任务取消]:{ae.InnerException}");
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    LogHelper.Trace($"[GetToken][{ex.GetType()}]:{ex.Message}");
                    return string.Empty;
                }
            }
        }

        public static string Md5(string pwd)
        {
            //初始化MD5对象
            MD5 md5 = MD5.Create();

            //将源字符串转化为byte数组
            Byte[] soucebyte = Encoding.Default.GetBytes(pwd);

            //soucebyte转化为mf5的byte数组
            Byte[] md5bytes = md5.ComputeHash(soucebyte);

            //将md5的byte数组再转化为MD5数组
            StringBuilder sb = new StringBuilder();
            foreach (Byte b in md5bytes)
            {
                //x表示16进制，2表示2位
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        //POST方法
        public static string HttpPost(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Encoding encoding = Encoding.UTF8;
                byte[] postData = encoding.GetBytes(postDataStr);
                request.ContentLength = postData.Length;
                Stream myRequestStream = request.GetRequestStream();
                myRequestStream.Write(postData, 0, postData.Length);
                myRequestStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
                string retString = myStreamReader.ReadToEnd();
                myStreamReader.Close();
                myResponseStream.Close();

                return retString;
            }
            catch (System.Net.WebException we)
            {
                qiyubrother.LogHelper.Trace($"WebException::{we.Message}");
                return string.Empty;
            }
            catch (System.IO.IOException ioe)
            {
                qiyubrother.LogHelper.Trace($"IOException::{ioe.Message}, {ioe.InnerException}");
                return string.Empty;
            }
            catch (Exception ex)
            {
                qiyubrother.LogHelper.Trace($"Exception::{ex.Message}, Type::{ex.GetType()}");
                return string.Empty;
            }
        }
    }
}
