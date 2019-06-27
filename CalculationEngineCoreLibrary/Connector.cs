using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
        private string currentHour = null;

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
            //string pattern = @"(?<=T.{3}).{2}";
            string pattern = @"(?<=T).{2}";
            Regex regex = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            channel.QueueDeclare(queue, true, false, false);
            Thread t = new Thread(new ThreadStart(d));
            t.Start();
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                DeviceMetric d = JsonConvert.DeserializeObject<DeviceMetric>(
                    System.Text.Encoding.Default.GetString(body));
                if (regex.Match(d.MetricDate).Groups[0].Value != currentHour)
                {
                    currentHour = regex.Match(d.MetricDate).Groups[0].Value;
                    //Console.WriteLine("Count: {0}", md.Count);
                    if (md.Count > 0)
                    {
                        //Console.WriteLine("########################################");
                        Thread persistT = new Thread(() => persist(md));
                        try
                        {
                            persistT.Start();
                            md.Clear();
                        }
                        catch (ThreadStateException e)
                        {
                            Console.WriteLine(e);
                            Console.WriteLine(persistT.ThreadState);
                            throw;
                        }
                        catch (InvalidOperationException e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        
                    }
                }
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
                
                //Console.WriteLine(regex.Match(d.MetricDate).Groups[0].Value);
                channel.BasicAck(ea.DeliveryTag, false);
            };
            string consumerTag = channel.BasicConsume("Test", false, consumer);
        }
        
        public void interupt()
        {
            Console.ReadLine();
            //Persist.persistData(md);
            Console.WriteLine("Count: {0}", md.Count);
            Console.WriteLine("Results");
            md.ToList().ForEach(x => Console.WriteLine("{0} : {1}", x.Key, x.Value.CValue));
            channel.Close();
            conn.Close();
        }

        public void persist(Dictionary<string, DeviceStats> dictionary)
        {
            Persist.persistData(dictionary);
        }

        public static int add(int a, int b)
        {
            return a + b;
        }
    }
}