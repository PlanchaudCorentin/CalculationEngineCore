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
        private int received = 0;

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
                    if (md.Count > 0)
                    {
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
                    d2.CValue = getMeanFromPreviousIteration(d2.CValue, d.MetricValue, d2.Iteration);
                    d2.Timestamp = d.MetricDate;
                    d2.Iteration++;
                }
                else
                {
                    md[String.Concat(d.MacAddress, d.DeviceType)] = 
                        new  DeviceStats(d.MacAddress, d.DeviceType, d.MetricValue, d.MetricDate);
                }
                received++;
                channel.BasicAck(ea.DeliveryTag, false);
            };
            string consumerTag = channel.BasicConsume(queue, false, consumer);
        }
        
        public void interupt()
        {
            Console.ReadLine();
            Console.WriteLine("Count: {0} messages received", received);
            channel.Close();
            conn.Close();
        }

        public void persist(Dictionary<string, DeviceStats> dictionary)
        {
            Persist.persistData(dictionary);
        }

        public static double getMeanFromPreviousIteration(double previousMean, double receivedValue, int iteration)
        {
            return Math.Round((previousMean * iteration + receivedValue) / (iteration + 1), 2);
        }
    }
}