using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CalculationEngineCoreLibrary
{
    public delegate void Del();
    public class Connector
    {
        private IConnection conn;
        private IModel channel;
        private Dictionary<string, DeviceStats> md;
        private Del d;

        public Connector(string user, string pass, string vhost, string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory
            {
                HostName = hostname,
                VirtualHost = vhost,
                Password = pass,
                UserName = user,
                Port = 5672
            };

            this.conn = factory.CreateConnection();
            this.md = new Dictionary<string, DeviceStats>();

            this.channel = conn.CreateModel();
            d = interupt;
        }

        public void startConsume(string queue)
        {
            channel.QueueDeclare(queue, true, false, false);
            Thread t = new Thread(new ThreadStart(d));
            t.Start();

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                DeviceMetric d = JsonConvert.DeserializeObject<DeviceMetric>(
                    System.Text.Encoding.Default.GetString(body));
                DeviceStats d2;
                if (md.TryGetValue(String.Concat(d.MacAddress, d.DeviceType), out d2))
                {
                    d2.CValue = Math.Round((d2.CValue + d.MetricValue) / 2, 2);
                    d2.Timestamp = d.MetricDate;
                }
                else
                {
                    md[String.Concat(d.MacAddress, d.DeviceType)] = 
                        new  DeviceStats(d.MacAddress, d.DeviceType, d.MetricValue, d.MetricDate);
                }
                
                Console.WriteLine("{0} {1} {2}", md[String.Concat(d.MacAddress, d.DeviceType)].MacAddress, 
                    md[String.Concat(d.MacAddress, d.DeviceType)].DeviceType,
                    md[String.Concat(d.MacAddress, d.DeviceType)].CValue);
                channel.BasicAck(ea.DeliveryTag, false);
            };
            string consumerTag = channel.BasicConsume("Test", false, consumer);
        }
        
        public void interupt()
        {
            Console.ReadLine();
            Persist.persistData(md);
            Console.WriteLine("Results");
            md.ToList().ForEach(x => Console.WriteLine("{0} : {1}", x.Key, x.Value.CValue));
            channel.Close();
            conn.Close();
        }
        
        public static int add(int a, int b)
        {
            return a + b;
        }
    }
}