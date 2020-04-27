
using BytesOperations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Text;
using UserSDK;

namespace RPC
{

    public class RpcUser
    {
        private readonly IConnection connection;
        private readonly IModel channel;
        private readonly string replyQueueName ;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<User> respQueue = new BlockingCollection<User>();

        private readonly IBasicProperties props;
        BytesOp bytes = new BytesOp();

        public RpcUser()
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
                var response = bytes.FromByteArray<User>(body.ToArray());
                if (ea.BasicProperties.CorrelationId == correlationId)
                {                   
                    respQueue.Add(response);
                }
            };
        }


        public User CallUser(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(
                exchange: "",
                routingKey: "server_user_queue",
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
