using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CalculationEngineCoreLibrary
{
    public class Connector
    {
        private IConnection conn;
        private IModel channel;

        public Connector(string user, string pass, string vhost, string hostname)
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = hostname;
            factory.VirtualHost = vhost;
            factory.Password = pass;
            factory.UserName = user;
            factory.Port = 5672;

            this.conn = factory.CreateConnection();

            this.channel = conn.CreateModel();
            channel.QueueDeclare("Test", true, false, false);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (ch, ea) =>
            {
                var body = ea.Body;
                Console.WriteLine(System.Text.Encoding.Default.GetString(body));
                channel.BasicAck(ea.DeliveryTag, false);
            };
            string consumerTag = channel.BasicConsume("Test", false, consumer);
            
        }

        public static int add(int a, int b)
        {
            return a + b;
        }
    }
}