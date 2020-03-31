using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class SocketHttpHelper
    {
        public static class HttpState
        {
            public const string S200 = "200";
        }
        /// <summary>
        /// POST 请求，没有 POST BODY
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="port"></param>
        /// <param name="url"></param>
        /// <param name="status"></param>
        /// <param name="data"></param>
        public static void SocketHttpPost(string hostName, string port, string url, out string status, out string data)
        {
            //得到主机信息
            IPHostEntry ipInfo = Dns.GetHostByName(hostName);
            //取得IPAddress[]
            IPAddress[] ipAddr = ipInfo.AddressList;
            //得到ip
            IPAddress ip = ipAddr[0];
            //组合出远程终结点
            IPEndPoint hostEP = new IPEndPoint(ip, Convert.ToInt32(port));
            //创建Socket 实例
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 60000); // 设置最大接收时间
            try
            {
                //尝试连接
                socket.Connect(hostEP);
            }
            catch (Exception se)
            {
                var __s = "连接错误：" + se.Message;
                LogHelper.OutputDebugString(__s);
                Console.WriteLine(__s);

                status = string.Empty;
                data = string.Empty;
                return;
            }
            //发送给远程主机的请求内容串
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri);
            var s = uri.IsAbsoluteUri ? uri.PathAndQuery : uri.OriginalString;

            string sendStr = $"POST {s} HTTP/1.1{Environment.NewLine}Host: {hostName}{Environment.NewLine}Content-Type: application/x-www-form-urlencoded{Environment.NewLine}Connection: close{Environment.NewLine}{Environment.NewLine}";
            //创建bytes字节数组以转换发送串
            byte[] bytesSendStr = new byte[1024];
            //将发送内容字符串转换成字节byte数组
            bytesSendStr = Encoding.ASCII.GetBytes(sendStr);
            try
            {
                //向主机发送请求
                socket.Send(bytesSendStr, bytesSendStr.Length, 0);
            }
            catch (Exception ce)
            {
                var __s = "发送错误：" + ce.Message;
                LogHelper.OutputDebugString(__s);
                Console.WriteLine(__s);
                status = string.Empty;
                data = string.Empty;
                return;
            }
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            //返回实际接收内容的字节数
            int bytes = 0;
            //循环读取，直到接收完所有数据
            while (true)
            {
                bytes = socket.Receive(recvBytes, recvBytes.Length, 0);
                //读取完成后退出循环
                if (bytes <= 0)
                    break;
                //将读取的字节数转换为字符串
                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
            }
            //将所读取的字符串转换为字节数组
            var sPos = recvStr.IndexOf(" ");
            var ePos = recvStr.IndexOf(Environment.NewLine);
            status = recvStr.Substring(sPos + 1, ePos - sPos).Split(' ')[0];
            if (status == "200")
            {
                var pos = recvStr.IndexOf($"{Environment.NewLine}{Environment.NewLine}");
                var subStr = recvStr.Substring(pos + Environment.NewLine.Length * 2);
                pos = subStr.IndexOf($"{Environment.NewLine}");
                var datalen = Convert.ToInt32(subStr.Substring(0, pos), 16);
                data = subStr.Substring(pos + Environment.NewLine.Length, datalen);
                // Console.WriteLine(recvStr);
            }
            else
            {
                data = string.Empty;
            }
            socket.Shutdown(SocketShutdown.Both);
            //关闭Socket
            socket.Close();
        }

        public static void SocketHttpPost(string hostName, string port, string url, string body, out string status, out string data)
        {
            //得到主机信息
            IPHostEntry ipInfo = Dns.GetHostByName(hostName);
            //取得IPAddress[]
            IPAddress[] ipAddr = ipInfo.AddressList;
            //得到ip
            IPAddress ip = ipAddr[0];
            //组合出远程终结点
            IPEndPoint hostEP = new IPEndPoint(ip, Convert.ToInt32(port));
            //创建Socket 实例
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //尝试连接
                socket.Connect(hostEP);
            }
            catch (Exception se)
            {
                var __s = "连接错误：" + se.Message;
                LogHelper.OutputDebugString(__s);
                Console.WriteLine(__s);

                status = string.Empty;
                data = string.Empty;
                return;
            }
            //发送给远程主机的请求内容串
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri);
            var s = uri.IsAbsoluteUri ? uri.PathAndQuery : uri.OriginalString;
            string sendStr = $"POST {s} HTTP/1.1{Environment.NewLine}Host: {hostName}{Environment.NewLine}Connection: close{Environment.NewLine}Content-Type: application/x-www-form-urlencoded{Environment.NewLine}Content-Length: {body.Length}{Environment.NewLine}{Environment.NewLine}{body}";
            //创建bytes字节数组以转换发送串
            byte[] bytesSendStr = new byte[1024];
            //将发送内容字符串转换成字节byte数组
            bytesSendStr = Encoding.ASCII.GetBytes(sendStr);
            try
            {
                //向主机发送请求
                socket.Send(bytesSendStr, bytesSendStr.Length, 0);
            }
            catch (Exception ce)
            {
                var __s = "发送错误：" + ce.Message;
                LogHelper.OutputDebugString(__s);
                Console.WriteLine(__s);
                status = string.Empty;
                data = string.Empty;
                return;
            }
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            //返回实际接收内容的字节数
            int bytes = 0;
            //循环读取，直到接收完所有数据
            while (true)
            {
                bytes = socket.Receive(recvBytes, recvBytes.Length, 0);
                //读取完成后退出循环
                if (bytes <= 0)
                    break;
                //将读取的字节数转换为字符串
                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
            }
            //将所读取的字符串转换为字节数组
            var sPos = recvStr.IndexOf(" ");
            var ePos = recvStr.IndexOf(Environment.NewLine);
            status = recvStr.Substring(sPos + 1, ePos - sPos).Split(' ')[0];
            if (status == "200")
            {
                var pos = recvStr.IndexOf($"{Environment.NewLine}{Environment.NewLine}");
                var subStr = recvStr.Substring(pos + Environment.NewLine.Length * 2);
                data = subStr;
                //pos = subStr.IndexOf($"{Environment.NewLine}");
                //var datalen = Convert.ToInt32(subStr.Substring(0, pos), 16);
                //data = subStr.Substring(pos + Environment.NewLine.Length, datalen);
                // Console.WriteLine(recvStr);
            }
            else
            {
                data = string.Empty;
            }
            socket.Shutdown(SocketShutdown.Both);
            //关闭Socket
            socket.Close();
        }


        public static void SocketHttpGet(string hostName, string port, string url, out string status, out string data)
        {
            //得到主机信息
            IPHostEntry ipInfo = Dns.GetHostByName(hostName);
            //取得IPAddress[]
            IPAddress[] ipAddr = ipInfo.AddressList;
            //得到ip
            IPAddress ip = ipAddr[0];
            //组合出远程终结点
            IPEndPoint hostEP = new IPEndPoint(ip, Convert.ToInt32(port));
            //创建Socket 实例
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //尝试连接
                socket.Connect(hostEP);
            }
            catch (Exception se)
            {
                var __s = "连接错误：" + se.Message;
                LogHelper.OutputDebugString(__s);
                Console.WriteLine(__s);

                status = string.Empty;
                data = string.Empty;
                return;
            }
            //发送给远程主机的请求内容串
            Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out Uri uri);
            var s = uri.IsAbsoluteUri ? uri.PathAndQuery : uri.OriginalString;

            string sendStr = $"GET {s} HTTP/1.1{Environment.NewLine}Host: {hostName}{Environment.NewLine}Connection: close{Environment.NewLine}{Environment.NewLine}";
            //创建bytes字节数组以转换发送串
            byte[] bytesSendStr = new byte[1024];
            //将发送内容字符串转换成字节byte数组
            bytesSendStr = Encoding.ASCII.GetBytes(sendStr);
            try
            {
                //向主机发送请求
                socket.Send(bytesSendStr, bytesSendStr.Length, 0);
            }
            catch (Exception ce)
            {
                var __s = "发送错误：" + ce.Message;
                LogHelper.OutputDebugString(__s);
                Console.WriteLine(__s);
                status = string.Empty;
                data = string.Empty;
                return;
            }
            string recvStr = "";
            byte[] recvBytes = new byte[1024];
            //返回实际接收内容的字节数
            int bytes = 0;
            //循环读取，直到接收完所有数据
            while (true)
            {
                bytes = socket.Receive(recvBytes, recvBytes.Length, 0);
                //读取完成后退出循环
                if (bytes <= 0)
                    break;
                //将读取的字节数转换为字符串
                recvStr += Encoding.ASCII.GetString(recvBytes, 0, bytes);
            }
            //将所读取的字符串转换为字节数组
            var sPos = recvStr.IndexOf(" ");
            var ePos = recvStr.IndexOf(Environment.NewLine);
            status = recvStr.Substring(sPos + 1, ePos - sPos).Split(' ')[0];
            if (status == "200")
            {
                var pos = recvStr.IndexOf($"{Environment.NewLine}{Environment.NewLine}");
                var subStr = recvStr.Substring(pos + Environment.NewLine.Length * 2);
                pos = subStr.IndexOf($"{Environment.NewLine}");
                var datalen = Convert.ToInt32(subStr.Substring(0, pos), 16);
                data = subStr.Substring(pos + Environment.NewLine.Length, datalen);
                // Console.WriteLine(recvStr);
            }
            else
            {
                data = string.Empty;
            }
            socket.Shutdown(SocketShutdown.Both);
            //关闭Socket
            socket.Close();
        }

    }
}
