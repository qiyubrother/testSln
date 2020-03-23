using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class LogHelper
    {
        static ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
        private static bool _enable = false;
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern void OutputDebugString(string message);

        /// <summary>
        /// 控制台输出字符串
        /// </summary>
        /// <param name="message"></param>
        public static void PrintMessage(string message)
        {
            var s = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]{message}";
             Console.WriteLine(s);
            //Task.Run(() => OutputDebugString(s));
        }
        /// <summary>
        /// 启动日志服务
        /// </summary>
        public static void StartService()
        {
            _enable = true;
            LogWriter();
        }
        /// <summary>
        /// 停止日志服务
        /// </summary>
        public static void Stop()
        {
            _enable = false;
        }
        /// <summary>
        /// 记录日志（保存到文件，同时调用OutputDebugString）
        /// </summary>
        /// <param name="s"></param>
        /// <param name="param"></param>
        public static void Trace(string s, params object[] param)
        {
            var p = param == null || param.Length == 0 ? new[] { "" } : param;
            var str = $"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]";
            if (param == null || param.Length == 0)
            {
                str = str + s;
            }
            else
            {
                str = str + string.Format(s, p);
            }
            //Console.WriteLine(str);
            queue.Enqueue(str);
        }
        /// <summary>
        /// 写日志到文件
        /// </summary>
        private static void LogWriter()
        {
            var th = new Thread(new ThreadStart(() =>
            {
                while (_enable)
                {
                    if (queue.TryDequeue(out string item))
                    {
                        var cnt = 0;
                        do
                        {
                            try
                            {
                                var fn = Path.Combine(Environment.CurrentDirectory, $"Log-{DateTime.Now.Year}{DateTime.Now.Month.ToString().PadLeft(2, '0')}{DateTime.Now.Day.ToString().PadLeft(2, '0')}.log");
                                File.AppendAllLines(fn, new[] { item });
                                OutputDebugString(item);
                                break;
                            }
                            catch
                            {
                                cnt++;
                                System.Threading.Thread.Sleep(200);
                            }
                            if (cnt > 3)
                            {
                                // 超过3次写入错误
                                var efn = $"Error-{DateTime.Now.Year}{DateTime.Now.Month.ToString().PadLeft(2, '0')}{DateTime.Now.Day.ToString().PadLeft(2, '0')}.log";
                                try
                                {
                                    System.IO.File.AppendAllLines(efn, new[] { $"[{DateTime.Now.ToString()}]日志系统错误。" });
                                }
                                catch { }

                                break;
                            }
                        } while (true);
                    }
                    System.Threading.Thread.Sleep(10);
                }
            }));
            th.IsBackground = true;
            th.Start();
        }

    }
}
