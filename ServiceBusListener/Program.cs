using Microsoft.ServiceBus.Messaging;
using System;
using System.Configuration;
using System.Runtime.Serialization;
using TestPhotoUploader.Models;

namespace ServiceBusListener
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.AppSettings["ServiceBusConnectionString"];
            string queueName = "photoanalyzingresults";

            QueueClient client = QueueClient.CreateFromConnectionString(connectionString, queueName);

            client.OnMessage(message =>
            {
                var entity = message.GetBody<PhotoAnalysisResult>(new DataContractSerializer(typeof(PhotoAnalysisResult)));
                Console.WriteLine($"Message was received: Description: {entity.Description}");
            });

            Console.WriteLine("Press ENTER to exit program");
            Console.ReadLine();
        }
    }
}
