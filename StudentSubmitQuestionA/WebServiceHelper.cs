using Microsoft.CSharp;
using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;

namespace qiyubrother
{
    public class WebServiceHelper
    {
        static System.Reflection.Assembly assembly = null;
        public static string CallMethod(string url, string methodName, object[] args)
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

            //生成代理实例，并调用方法
            assembly = resource_cr.CompiledAssembly;

            //生成代理实例，并调用方法
            Type t = assembly.GetType("client" + "." + sd.Services[0].Name, true, true);
            object obj = Activator.CreateInstance(t);

            System.Reflection.MethodInfo mi = t.GetMethod(methodName);
            //注：method.Invoke(o, null)返回的是一个Object,如果你服务端返回的是DataSet,这里也是用(DataSet)method.Invoke(o, null)转一下就行了,method.Invoke(0,null)这里的null可以传调用方法需要的参数,string[]形式的
            return (string)mi.Invoke(obj, args);
        }

        public static object GetObject(string url)
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

            //生成代理实例，并调用方法
            assembly = resource_cr.CompiledAssembly;

            //生成代理实例，并调用方法
            Type t = assembly.GetType("client" + "." + sd.Services[0].Name, true, true);

            return Activator.CreateInstance(t);
        }

        public static string CallMethod(object obj, string methodName, object[] args)
        {
            MethodInfo mi = obj.GetType().GetMethod(methodName);
            
            return (string)mi.Invoke(obj, args);
        }
    }
}
