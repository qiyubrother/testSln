using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qiyubrother
{
    public class MQHelper
    {
        public static IConnection connection;
        public static IConnection CreateMqConnection(string mqIp, string mqPort, string mqUserName, string mqPassword)
        {
            try
            {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = mqIp;
                factory.Port = Convert.ToInt32(mqPort);
                factory.UserName = mqUserName;
                factory.Password = mqPassword;
                connection = factory.CreateConnection();
            }
            catch (Exception ex)
            {
                LogHelper.Trace("CreateConnection failed. {0},{1},{2}", ex.Message, ex.Source, ex.StackTrace);
            }
            return connection;
        }

        /// 发送MQ消息给服务器
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="msg"></param>
        public static void sentMsgToMQqueue(string queueName, string msg)
        {
            try
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: queueName,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    //var args = new Dictionary<string, object>();
                    //args.Add("x-message-ttl", 60000);
                    //channel.QueueDeclare(queueName, false, false, false, args);

                    var body = Encoding.UTF8.GetBytes(msg);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: string.Empty,
                                         routingKey: queueName,
                                         basicProperties: properties,
                                         body: body);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace($"sentMsgToMQque failed.{ex.Message},{ex.StackTrace},{ex.Source}");
            }
        }

        public static void sentMsgToMQ(string routingKey, string msg, string serverExchangeName)
        {
            try
            {
                using (IModel channel = connection.CreateModel())
                {
                    // 生成随机队列名称
                    var queueName = channel.QueueDeclare().QueueName;
                    //使用topic exchange type，指定exchange名称
                    channel.ExchangeDeclare(exchange: serverExchangeName, type: "topic");
                    var body = Encoding.UTF8.GetBytes(msg);
                    LogHelper.Trace($"发送MQ消息:{serverExchangeName}:{routingKey}>>>\n{msg}");
                    channel.BasicPublish(exchange: serverExchangeName, routingKey: routingKey, basicProperties: null, body: body);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Trace($"Exception::sentMsgToMQque:{ex.Message},{ex.StackTrace},{ex.Source}");
            }
        }

        public static void sentMsgToMQ(string routingKey, IModel channel, string queueName, string msg, string serverExchangeName)
        {
            try
            {
                //使用topic exchange type，指定exchange名称
                channel.ExchangeDeclare(exchange: serverExchangeName, type: "topic");
                var body = Encoding.UTF8.GetBytes(msg);
                LogHelper.Trace($"发送MQ消息:{serverExchangeName}:{routingKey}>>>\n{msg}");
                channel.BasicPublish(exchange: serverExchangeName, routingKey: routingKey, basicProperties: null, body: body);
            }
            catch (Exception ex)
            {
                LogHelper.Trace($"sentMsgToMQque:{ex.Message},{ex.StackTrace},{ex.Source}");
            }
        }
    }
}
