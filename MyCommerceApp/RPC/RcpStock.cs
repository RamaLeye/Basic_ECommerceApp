using BytesOperations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockSDK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace RPC
{
    public class RcpStock
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<ItemLine> respQueue = new BlockingCollection<ItemLine>();
        private readonly BlockingCollection<string> ProductsQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;
        BytesOp bytes = new BytesOp();

        public RcpStock()
        {

            var factory = new ConnectionFactory() { HostName = "localhost" };

            connection = factory.CreateConnection();
            channel = connection.CreateModel();
            replyQueueName = channel.QueueDeclare().QueueName;
            consumer = new EventingBasicConsumer(channel);
            props = channel.CreateBasicProperties();
            var correlationId = Guid.NewGuid().ToString();
            props.CorrelationId = correlationId;
            props.ReplyTo = replyQueueName;

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var response2 = Encoding.UTF8.GetString(body.ToArray());

                if (ea.BasicProperties.CorrelationId == correlationId)
                {
                    if (response2.Contains("PRODUCTS"))
                    {
                        ProductsQueue.Add(response2);
                    }
                    else
                    {
                        var response1 = bytes.FromByteArray<ItemLine>(body.ToArray());
                        respQueue.Add(response1);
                        Console.WriteLine(" \n [*] Item added in your cart ..");
                    }
                }
            };  
        }


        public string CallStock(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "server_stock_queue",
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            Console.WriteLine(" [Queue taken] : ");

            return ProductsQueue.Take();
        }

        public ItemLine CallItemLine(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "server_stock_queue",
                basicProperties: props,
                body: messageBytes);

            channel.BasicConsume(
                consumer: consumer,
                queue: replyQueueName,
                autoAck: true);

            return respQueue.Take();
        }

        public void Close()
        {
            connection.Close();

        }


    }

}

