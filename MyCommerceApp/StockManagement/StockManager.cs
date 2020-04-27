using BytesOperations;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StockSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;

namespace StockManagement
{
    class StockManager
    {

        List<ItemLine> listItemsCurrent;
        List<ItemLine> listItems;
        string affiche = "";
        private const string ItemsFolder = @"products\";


        public StockManager()
        {
            listItemsCurrent = LoadItems();
            affiche = affichageCurrentListe();
        }

        public List<ItemLine> LoadItems()
        {
            listItems = new List<ItemLine>();

            string[] files = Directory.GetFiles(@".\" + ItemsFolder, "*.json");

            foreach (string file in files)
            {
                string json = File.ReadAllText(file);
                var temporaryItemList = JsonConvert.DeserializeObject<List<ItemLine>>(json);

                for (int i = 0; i < temporaryItemList.Count; i++)
                {
                    listItems.Add(temporaryItemList[i]);
                }
            }
            return listItems;
        }


        public string affichageCurrentListe()
        {
            string affichage = "";
            affichage += String.Format(" \n|{0,30}|{1,10}|{2,10}|", "   Article                        ", "Unit price", "Quantity");
            for (int i = 0; i < listItemsCurrent.Count; i++)
            {
                string line = "\n" + i + " - " + listItemsCurrent[i].ToString();
                affichage += line;
            }
            return affichage;
        }


        public int quantityByName(string name)
        {
            int quantity = 0;
            foreach (ItemLine itemline in listItems)
            {
                if (itemline.getName().Equals(name))
                {
                    quantity = itemline.getQuantiy();
                } 
            }
            return quantity;
        }
        

        public ItemLine ReserveItem(int quantity, string name)
        {
            ItemLine itemLine = new ItemLine();

            foreach(ItemLine itemline in listItemsCurrent)
            {
                if (itemline.getName().Equals(name))
                {
                    int currentQuantity = itemline.getQuantiy();
                    itemLine = itemline;

                    if (quantity >= currentQuantity)
                    {
                        itemline.setQuantity(0);
                    }
                    else
                    {
                        itemline.setQuantity(currentQuantity - quantity);         
                    }
                }
            }
            return itemLine;
        }


        public void ReleaseItem(ItemLine line)
        {   
            foreach (ItemLine itemline in listItemsCurrent)
            {
                if (itemline.getName().Equals(line.getName()))
                {
                    int currentQuantity = itemline.getQuantiy();
                    itemline.setQuantity(currentQuantity + line.getQuantiy());
                }
            }
      
        }


        public string getNameByIndex(int index)
        {
            return listItems[index].getName();
        }


        public static void Main()
        {

            StockManager stockManager = new StockManager();
          
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {

                channel.QueueDeclare(queue: "server_stock_queue", durable: false,
                  exclusive: false, autoDelete: false, arguments: null);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume(queue: "server_stock_queue",
                  autoAck: false, consumer: consumer);
                Console.WriteLine(" [x] Awaiting RPC stock requests");
               

                consumer.Received += (model, ea) =>
                {
                   
                    ItemLine response = new ItemLine();
                    string response2 = null;

                    var body = ea.Body;
                    var props = ea.BasicProperties;
                    var replyProps = channel.CreateBasicProperties();
                    replyProps.CorrelationId = props.CorrelationId;

                    try
                    {
                        var message = Encoding.UTF8.GetString(body.ToArray());

                        if (message.Equals("products"))
                        {
                            response2 = "PRODUCTS \n" + stockManager.affichageCurrentListe();
                        }
                        else
                        {
                            string[] numbers = Regex.Split(message, @"\D+");
                            string name = stockManager.getNameByIndex(int.Parse(numbers[0]));
                            int quantity = int.Parse(numbers[1]);
                            int maxQuantity = stockManager.quantityByName(name);
                            ItemLine reserved = stockManager.ReserveItem(quantity, name);

                            if (quantity >= maxQuantity)
                            {
                                response = new ItemLine(reserved.getName(), reserved.getUnitPrice(), maxQuantity);
                            }

                            else
                            {
                                response = new ItemLine(reserved.getName(), reserved.getUnitPrice(), quantity);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(" [error] " + e.Message);
                        response = null;
                    }
                    finally
                    {
                        Console.WriteLine(" [....] " + " Trying to deliver " + response.getName());
                        if(response2 != null)
                        {
                            var responseBytes = Encoding.UTF8.GetBytes(response2);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                              basicProperties: replyProps, body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag,
                              multiple: false);
                            Console.WriteLine(" [..OK...] " + " Liste products SENT  ");
                        }

                        else
                        {
                            var responseBytes = ToByteArray<ItemLine>(response);
                            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                              basicProperties: replyProps, body: responseBytes);
                            channel.BasicAck(deliveryTag: ea.DeliveryTag,
                              multiple: false);
                            Console.WriteLine(" [..OK...] " + " Item SENT ");
                        }
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
