/*

  测试学生列表

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using Microsoft.CSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.IO;
using System.Net;
using System.Threading;

namespace WebAPIConsoleTest
{
    class Program
    {
        static System.Reflection.Assembly assembly = null;
        static string resource_classname = string.Empty;
        static async Task Main(string[] args)
        {
            //设定编译参数
            CompilerParameters cplist = new CompilerParameters();
            cplist.GenerateExecutable = false;//动态编译后的程序集不生成可执行文件
            cplist.GenerateInMemory = true;//动态编译后的程序集只存在于内存中，不在硬盘的文件上
            cplist.ReferencedAssemblies.Add("System.dll");
            cplist.ReferencedAssemblies.Add("System.XML.dll");
            cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
            cplist.ReferencedAssemblies.Add("System.Data.dll");
            long t2 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
            //获取WSDL
            WebClient wc = new WebClient();
            //Stream stream = wc.OpenRead(url + "?WSDL");
            Stream stream = wc.OpenRead("http://192.168.10.168:8088/ws/IWsResourceService?wsdl");
            ServiceDescription sd = ServiceDescription.Read(stream);
            //var preclass_classname = sd.Services[0].Name;
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, "", "");
            CodeNamespace cn = new CodeNamespace("client");
            //生成客户端代理类代码
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            CSharpCodeProvider csc = new CSharpCodeProvider();
            //编译代理类
            var preclass_cr = csc.CompileAssemblyFromDom(cplist, ccu);

            //编译代理类
            var resource_cr = csc.CompileAssemblyFromDom(cplist, ccu);

            resource_classname = sd.Services[0].Name;
            //生成代理实例，并调用方法
            assembly = resource_cr.CompiledAssembly;

            var r = new Random((int)DateTime.Now.ToFileTimeUtc());
            var counter = 0;
            Parallel.For(0, 10000, new Action<int>((i) => {
                Parallel.Invoke(new Action[]
                {
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                    ()=>{
                        var f = r.Next(0, 10);
                        var t = r.Next(f, 10 + r.Next(0, 10));
                        Console.WriteLine($"{f} -> {t}, Counter:{counter++}");
                        var s = WSgetStudentSpeakList(1, "a5e94b9e01e144318c5a543cae011eed, 9c9cfdd9ca1e4ec0978871d3f4ab88bc", f, t);
                    },
                }); ;
            }));

            Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}]Task finished.");

        }
        public static readonly Mutex RefreshTokenMutex = new Mutex();
        public static async Task<string> WSgetStudentSpeakList(int times, string classID, int pageNum, int perNum)
        {
            var resource_server = "http://192.168.10.168:8088/ws/IWsResourceService?wsdl";
            var access_tocken = "5331ceea6eeb78824062d3a3480b210e";
            var refresh_tocken = "d57e737748d2662b525516ab27096dd2";
            var teachingPlanId = "f441cb26f5c24ac98d907b2cd7b1e9e0";
            var timeTableId = "05ecade4ab9b449fb4de5f01252a25af";
            string url = resource_server;
            string method = "getStudentSpeakList";
            string NowAccess_tocken = access_tocken;
            string NowRefreshToken = refresh_tocken;

            JObject obj = new JObject();
            JObject jObj = JObject.Parse(JsonConvert.SerializeObject(obj));
            jObj.Add(new JProperty("Course_ID", teachingPlanId));
            jObj.Add(new JProperty("Class_IDS", classID));
            //jObj.Add(new JProperty("Class_ID", classID));
            jObj.Add(new JProperty("Time_Table_ID", timeTableId));
            jObj.Add(new JProperty("accessToken", NowAccess_tocken));
            jObj.Add(new JProperty("pageNumber", pageNum));
            jObj.Add(new JProperty("pageSize", perNum));
            string[] param = new string[] { jObj.ToString() };

            try
            {
                string str_res = (string)doInvokeWebServiceResourceServer(url, method, param);
                //Console.WriteLine("{0} return:{1}", method, str_res);
                if (string.IsNullOrEmpty(str_res))
                {
                    Console.WriteLine("{0}:请求结果为空!", method);
                    return "";
                }
                JObject jo = (JObject)JsonConvert.DeserializeObject(str_res);
                string status = jo["err_code"].ToString();
                if (status == "200")
                {
                    return str_res;
                }
                else if (status == "2001")
                {
                    return "";
                }
                else if (status == "20002")
                {
                    if (times == 1)
                    {
                        string resStr = "";
                        RefreshTokenMutex.WaitOne();
                        if ((NowRefreshToken == refresh_tocken) && (NowAccess_tocken == access_tocken))
                        {
                            ; // 
                        }
                        resStr = await WSgetStudentSpeakList(2, classID, pageNum, perNum);
                        RefreshTokenMutex.ReleaseMutex();
                        return resStr;
                    }
                    else if (times == 2)
                    {
                        //CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20001")
                {
                    //如果refresh_token更新了access_tocken，重新请求数据
                    if (times == 1)
                    {
                        return await WSgetStudentSpeakList(2, classID, pageNum, perNum);
                    }
                    else if (times == 2)
                    {
                        //CodeHelper.ApplacationRestart(status);
                    }
                }
                else if (status == "20102")
                {
                    //CodeHelper.ApplacationRestart(status);
                }
                else
                {
                    //LogHelper.Trace("{0}：请求成功，返回了错误码【{1}】", method, status);
                    return "";
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Trace("{0}:{1},{2},{3}", method, ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
            finally
            {
                ;
            }
            return "";
        }

        public static object doInvokeWebServicePreClassServer(string url, string methodname, object[] args)
        {
            try
            {
                //设定编译参数
                CompilerParameters cplist = new CompilerParameters();
                cplist.GenerateExecutable = false;//动态编译后的程序集不生成可执行文件
                cplist.GenerateInMemory = true;//动态编译后的程序集只存在于内存中，不在硬盘的文件上
                cplist.ReferencedAssemblies.Add("System.dll");
                cplist.ReferencedAssemblies.Add("System.XML.dll");
                cplist.ReferencedAssemblies.Add("System.Web.Services.dll");
                cplist.ReferencedAssemblies.Add("System.Data.dll");
                long t2 = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
                //获取WSDL
                WebClient wc = new WebClient();
                //Stream stream = wc.OpenRead(url + "?WSDL");
                Stream stream = wc.OpenRead(url);
                ServiceDescription sd = ServiceDescription.Read(stream);
                var preclass_classname = sd.Services[0].Name;
                ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
                sdi.AddServiceDescription(sd, "", "");
                CodeNamespace cn = new CodeNamespace("client");
                //生成客户端代理类代码
                CodeCompileUnit ccu = new CodeCompileUnit();
                ccu.Namespaces.Add(cn);
                sdi.Import(cn, ccu);
                CSharpCodeProvider csc = new CSharpCodeProvider();
                //编译代理类
                var preclass_cr = csc.CompileAssemblyFromDom(cplist, ccu);

                //生成代理实例，并调用方法
                System.Reflection.Assembly assembly = preclass_cr.CompiledAssembly;
                Type t = assembly.GetType("client" + "." + preclass_classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了,method.Invoke(0,null)这里的null可以传调用方法需要的参数,string[]形式的
                return (string)mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                Console.WriteLine("doInvokeWebServicePreClassServer:{0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
                return "";
            }
        }

        public static object doInvokeWebServiceResourceServer(string url, string methodname, object[] args)
        {
            try
            {

                Type t = assembly.GetType("client" + "." + resource_classname, true, true);
                object obj = Activator.CreateInstance(t);
                System.Reflection.MethodInfo mi = t.GetMethod(methodname);
                //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了,method.Invoke(0,null)这里的null可以传调用方法需要的参数,string[]形式的
                return (string)mi.Invoke(obj, args);
            }
            catch (Exception ex)
            {
                //LogHelper.Trace("doInvokeWebServiceResourceServer:{0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
                Console.WriteLine(ex.Message);
                return "";
            }
        }
    }
}
