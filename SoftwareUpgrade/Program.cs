using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qiyubrother;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using fastJSON;
namespace SoftwareUpgrade
{
    class Program
    {
        static void Main(string[] args)
        {
            string ip = "192.168.10.168";
            string port = "8003";
            string account = "ywl_1";
            string pwd = TokenHelper.Md5("123456");
            var token = TokenHelper.GetToken(ip, port, account, pwd);
            Console.WriteLine(token);
            string url = "http://192.168.10.168:8088/ws/IEasySecServer?wsdl";
            var jo = new JObject();
            jo["ApplocationType"] = "1";
            jo["accessToken"] = token;
            var param = jo.ToString();
            //var rtn = WebServiceHelper.CallMethod(url, "getSoftwareNewest", new[] { param });
            var rtn = WebServiceHelper.CallMethod(url, "getSoftwareNewest", new[] { "1", token });
            Console.WriteLine(rtn);
            Console.ReadLine();

        }
    }
}
