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
        public static string GetToken(string ip, string port, string account, string pwd, int timeout = 10, object tag = null)
        {
            LogHelper.Trace($"[B]Account:{account}, Tag:{tag}");
            LogHelper.Trace($"[B]GetCode:{account}, Tag:{tag}");
            GetCode(ip, port, account, pwd, out string code, out string state, tag);
            LogHelper.Trace($"[E]GetCode:{account}, Tag:{tag}");
            var rtn = code == string.Empty ? string.Empty : GetToken(ip, port, account, code, state, "authorization_code", timeout, tag);
            LogHelper.Trace($"[E]Account:{account}, Tag:{tag}, AccessToken:{rtn}");

            return rtn;
        }

        private static void GetCode(string ip, string port, string account, string pwd, out string code, out string state, object tag)
        {
            var s = string.Empty;
            try
            {
                var url = $"http://{ip}:{port}/oauth2/login.html?client_id=1&response_type=code&username={account}&password={pwd}";
                //var url = $"http://www.baidu.com";
                s = HttpPostAsync(url, string.Empty).Result;
                JObject jo = (JObject)JsonConvert.DeserializeObject(s);
                if (jo["msgCode"].ToString() == "200")
                {
                    code = jo["code"].ToString();
                    state = jo["state"].ToString();
                }
                else
                {
                    code = string.Empty;
                    state = string.Empty;
                    LogHelper.Trace($"[Tag:{tag}][GetCode] failed. Return Message:{s}");
                }
            }
            catch(NullReferenceException nfe)
            {
                LogHelper.Trace($"[Tag:{tag}][GetCode][NullReferenceException]:{nfe.InnerException}");
                code = string.Empty;
                state = string.Empty;
            }
            catch (WebException we)
            {
                LogHelper.Trace($"[Tag:{tag}][GetCode][WebException]:{we.Message}");
                code = string.Empty;
                state = string.Empty;
            }
            catch (IOException ioe)
            {
                LogHelper.Trace($"[Tag:{tag}][GetCode][IOException]:{ioe.Message}, {ioe.InnerException}");
                code = string.Empty;
                state = string.Empty;
            }
            catch (JsonReaderException jre)
            {
                //LogHelper.Trace($"[Tag:{tag}][GetCode][Rtn]{s}");
                LogHelper.Trace($"[Tag:{tag}][GetCode][JsonReaderException]:{jre.Message}, {jre.InnerException}");
                code = string.Empty;
                state = string.Empty;
            }
            catch (Exception ex)
            {
                LogHelper.Trace($"[Tag:{tag}][GetCode][{ex.GetType()}]:{ex.Message}");
                code = string.Empty;
                state = string.Empty;
            }
        }
        /// <summary>
        /// 通过Code+State获取AccessToken
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="account"></param>
        /// <param name="code"></param>
        /// <param name="state"></param>
        /// <param name="grantType"></param>
        /// <param name="timeout">最长等待响应时间</param>
        /// <returns></returns>
        private static string GetToken(string ip, string port, string account, string code, string state, string grantType, int timeout, object tag)
        {
            string client_secret = "111";
            string baseUrl = "http://" + ip + ":" + port;
            string tokenUrl = baseUrl + "/oauth2/token";
            string redirect_uri = baseUrl + "/oauth2/getToken.do";

            using (HttpClient httpClient = new HttpClient())
            {
                int timout_sec = timeout; // 最长等待时间（秒）
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
                try
                {
                    var response = httpClient.PostAsync(tokenUrl, httpContent).Result;
                    response.EnsureSuccessStatusCode();
                    string responseValue = response.Content.ReadAsStringAsync().Result;

                    JObject jot = (JObject)JsonConvert.DeserializeObject(responseValue);
                    string err_code = jot["err_code"].ToString();
                    if (err_code == "200")
                    {
                        //LogHelper.Trace($"access_token:{jot["access_token"]}...");
                        return jot["access_token"].ToString();
                    }
                    else
                    {
                        LogHelper.Trace($"[Tag:{tag}][GetToken]err_code:{err_code}");
                        return string.Empty;
                    }
                }
                catch (AggregateException ae)
                {
                    LogHelper.Trace($"[Tag:{tag}][GetToken][AggregateException][响应超时导致的请求任务取消]:{ae.InnerException}");
                    return string.Empty;
                }
                catch (Exception ex)
                {
                    LogHelper.Trace($"[Tag:{tag}][GetToken][{ex.GetType()}]:{ex.Message}");
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
        public static async Task<string> HttpPostAsync(string Url, string postDataStr)
        {
            //try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.KeepAlive = false;
                //request.Proxy = null;
                request.ServicePoint.ConnectionLimit = 10000;
                request.ServicePoint.UseNagleAlgorithm = false;
                request.AllowWriteStreamBuffering = false;
                //request.ServicePoint.Expect100Continue = false;


                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Encoding encoding = Encoding.UTF8;
                byte[] postData = encoding.GetBytes(postDataStr);
                request.ContentLength = postData.Length;
                var myRequestStream = request.GetRequestStream();
                myRequestStream.Write(postData, 0, postData.Length);
                myRequestStream.Close();
                using (var response = await request.GetResponseAsync() as HttpWebResponse) // 慢
                {
                    Stream myResponseStream = response.GetResponseStream();
                    StreamReader myStreamReader = new StreamReader(myResponseStream, encoding);
                    string retString = myStreamReader.ReadToEnd();
                    myStreamReader.Close();
                    myResponseStream.Close();

                    return retString;
                }
            }
            //catch (System.Net.WebException we)
            //{
            //    qiyubrother.LogHelper.Trace($"WebException::{we.Message}");
            //    return string.Empty;
            //}
            //catch (System.IO.IOException ioe)
            //{
            //    qiyubrother.LogHelper.Trace($"IOException::{ioe.Message}, {ioe.InnerException}");
            //    return string.Empty;
            //}
            //catch (Exception ex)
            //{
            //    qiyubrother.LogHelper.Trace($"Exception::{ex.Message}, Type::{ex.GetType()}");
            //    return string.Empty;
            //}
        }
    }
}
