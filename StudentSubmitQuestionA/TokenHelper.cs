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
            return get_tokenAsync(ip, port, account, code, state, "authorization_code");
        }

        private static void GetCode(string ip, string port, string account, string pwd, out string code, out string state)
        {
            var url = $"http://{ip}:{port}/oauth2/login.html?client_id=1&response_type=code&username={account}&password={pwd}";
            var s = HttpPost(url, string.Empty);
            JObject jo = (JObject)JsonConvert.DeserializeObject(s);
            code = jo["code"].ToString();
            state = jo["state"].ToString();
        }

        private static string get_tokenAsync(string ip, string port, string account, string code, string state, string grantType)
        {
            int login_times = 0;
            login_times++;
            string client_secret = "111";

            string BaseUrl = "http://" + ip + ":" + port;

            String TokenUrl = BaseUrl + "/oauth2/token";
            String redirect_uri = BaseUrl + "/oauth2/getToken.do";

            using (HttpClient httpClient = new HttpClient())
            {
                int timout_sec = 2;
                httpClient.Timeout = TimeSpan.FromSeconds(timout_sec);
                var refreshToken = string.Empty;
                var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"client_id", "1"},
                    {"client_secret", client_secret},
                    {"refresh_token", refreshToken},
                    {"username", account},
                    {"redirect_uri", redirect_uri},
                    {"code", code},
                    {"state", state},
                    {"grant_type",grantType}
                });

                try
                {
                    var response = httpClient.PostAsync(TokenUrl, httpContent).Result;
                    response.EnsureSuccessStatusCode();
                    string responseValue = response.Content.ReadAsStringAsync().Result;

                    JObject jot = (JObject)JsonConvert.DeserializeObject(responseValue);
                    string err_code = jot["err_code"].ToString();
                    if (err_code == "200")
                    {
                        return jot["access_token"].ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                catch (Exception ex)
                {
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
