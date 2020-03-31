/*
 * 在多个AppDomain中运行程序
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MutiDomainApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 输出当前应用程序域名称
            Console.WriteLine($"{Thread.GetDomain().FriendlyName}");
            // 创建AppDomain
            var domainX = AppDomain.CreateDomain($"DomainX");
            // 创建DomainX对应的类的实例
            var work = (CWorkDomain)domainX.CreateInstanceAndUnwrap(Assembly.GetAssembly(typeof(CWorkDomain)).FullName, typeof(CWorkDomain).FullName);
            // 在DomainX中运行实例
            work.Run();
            Console.ReadLine();
        }
    }

    /// <summary>
    /// 工作域
    /// 必须支持序列号，并且派生自MarshalByRefObject类
    /// </summary>
    [Serializable]
    public class CWorkDomain : MarshalByRefObject
    {
        public void Run()
        {
            Console.WriteLine($"{Thread.GetDomain().FriendlyName}");
        }
    }
}
