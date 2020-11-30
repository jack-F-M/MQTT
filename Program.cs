using MQTTnet;
using MQTTnet.Core;
using MQTTnet.Core.Client;
using MQTTnet.Core.Packets;
using MQTTnet.Core.Protocol;
using MQTTnet.Core.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using Newtonsoft.Json;
using System.Linq;

namespace mqttStart
{
    class Program
    {

        static void Main(string[] args)
        {
            //服务器端建立连接建立连接
            //var options = new MqttServerOptions()
            //{
            //    ConnectionValidator = p =>
            //    {
            //        if (p.ClientId.Length < 10)
            //        {
            //            return MqttConnectReturnCode.ConnectionRefusedIdentifierRejected;
            //        }
            //        return MqttConnectReturnCode.ConnectionAccepted;
            //    }
            //};
            //var mqttServer = new MqttServerFactory().CreateMqttServer(options);
            //mqttServer.StartAsync().GetAwaiter().GetResult();

            //客户端建立连接
            var options1 = new MqttClientTcpOptions
            {
                Server = "127.0.0.1",
                UserName = "admin",
                Password = "public",
                Port = 18083,
                CleanSession = false,
                KeepAlivePeriod =  TimeSpan.FromSeconds(100)
            };

            //建立客户端连接
            var client = new MqttClientFactory().CreateMqttClient();
            client.ConnectAsync(options1);

            //生成客户端ID并连接服务器
            string clientId = Guid.NewGuid().ToString();


            //// 订阅主题"/home/temperature" 消息质量为 2 
            var messgae = client.SubscribeAsync(new List<TopicFilter> {
                new TopicFilter("家/客厅/空调/#", MqttQualityOfServiceLevel.AtMostOnce)
            }).GetAwaiter().GetResult();

            foreach (var item in messgae)
            {
                //item.ReturnCode
            }

            User user = new User();

            string json = JsonConvert.SerializeObject(user);

            user = JsonConvert.DeserializeObject<User>(json);


            var appMsg = new MqttApplicationMessage("家/客厅/空调/开关", Encoding.UTF8.GetBytes("消息内容"), MqttQualityOfServiceLevel.AtMostOnce, false);
            // 发布消息到主题 "/home/temperature" 消息质量为 2,不保留 
            client.PublishAsync(appMsg);

            ///////////////////////////db

            string str = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4};",
                                       "localhost", 3306, "wisdompurchase", "root", "1234");
            DbConnection conn = new SqlConnection(str);    //创建连接
                                                           //连接的数据库名称
            List<User> list = conn.Query<User>("select * from user").ToList();

            int count = conn.Execute("insert xxx value(,,,,)", list);
            Console.ReadKey();

        }


    }

    public class User
    {

    }
}
