using Mango.Services.EmailAPI.Models.Dto;
using Mango.Services.EmailAPI.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.EmailAPI.Messaging
{
    public class RabbitMQCartConsumer : BackgroundService
    {
        private readonly string emailCartQueue;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private IConnection _connection;
        private IModel _channel;
        
        public RabbitMQCartConsumer(IConfiguration configuration, IEmailService emailService)
        {
            _configuration = configuration;
            _emailService = emailService;
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                Password = "guest",
                UserName = "guest",
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            emailCartQueue = _configuration.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue");

            _channel.QueueDeclare(emailCartQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ch, ea) =>
            {
                var content = Encoding.UTF8.GetString(ea.Body.ToArray());
                CartDto? cartDto = JsonConvert.DeserializeObject<CartDto>(content);
                HandleMessage(cartDto).GetAwaiter().GetResult();

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            };

            _channel.BasicConsume(emailCartQueue, autoAck: false, consumer);

            return Task.CompletedTask;
        }

        private async Task HandleMessage(CartDto? cartDto)
        {
            _emailService.EmailCartAndLog(cartDto).GetAwaiter().GetResult();
        }
    }
}
