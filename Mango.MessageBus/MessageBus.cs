using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus
{
    public class AzureMessageBus : IAzureMessageBus
    {
        private string connectionString = "Endpoint=sb://cintrashopweb.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=RXpSqpoHjBMU+8g3UAmCEJFNxkWNsICys+ASbBuiL+A=";

        public async Task PublishMessage(object message, string topic_queue_name)
        {
            await using var client = new ServiceBusClient(connectionString);

            ServiceBusSender sender = client.CreateSender(topic_queue_name);

            var jsonMessage = JsonConvert.SerializeObject(message);

            ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage))
            {
                CorrelationId = Guid.NewGuid().ToString(),
            };

            await sender.SendMessageAsync(finalMessage);
            await client.DisposeAsync();
        }
    }
}
