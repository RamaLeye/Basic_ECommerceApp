using BytesOperations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UserSDK;

namespace UserManagement
{
    public class UserManager
    {

        private const string UserFolder = @"users\";
        BytesOp bytes;
       List<User> listUsers { get; set; }


        public  UserManager()
        {
            listUsers = new List<User>();
            LoadUsers();
        }


        public  List<User> LoadUsers()
        {
            List<User> listeLoaded = new List<User>();
            string[] files = Directory.GetFiles(@".\" + UserFolder, "*.json");

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                var temporaryUserList = JsonConvert.DeserializeObject<List<User>>(json);
                foreach (User user in temporaryUserList)
                {
                    listeLoaded.Add(user);
                }
            }
            listUsers = listeLoaded;
            return listeLoaded;
        }


        public User getByUsername(string username)
        {
            User u = new User();
            Console.WriteLine(" liste = " + listUsers.Count);          
            for (int iterateurUser = 0; iterateurUser < listUsers.Count; iterateurUser++)
            {      
                if (listUsers[iterateurUser].getUserName().Equals(username))
                {
                    u =  listUsers[iterateurUser];
                 }
            }
            Console.WriteLine(" \n [*] Username checked.");
            return u;
        }


        public static void Main()
        {
            UserManager userManager = new UserManager();

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                
                channel.QueueDeclare(queue: "server_user_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "server_user_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC user requests");

                consumer.Received += (model, ea) =>
                {
                    User response = new User();
                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());
                        Console.WriteLine(" [.] Checking user ({0}) ..................................... ", message);
                        response = userManager.getByUsername(message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [error] " + e.Message);
                        response = null;
                    }
                    finally
                    {
                        var responseBytes = ToByteArray<User>(response);
                        channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                          basicProperties: replyProps, body: responseBytes);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag,
                          multiple: false);
                        Console.WriteLine(" [*] " + "Delivered");
                    }
                };
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

    }
}

