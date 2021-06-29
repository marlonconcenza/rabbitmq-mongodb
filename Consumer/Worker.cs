using Common.Models;
using Common.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider, IUserRepository userRepository)
        {
            _logger = logger;

            _serviceProvider = serviceProvider;
            _userRepository = userRepository;

            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq"
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                        queue: "users", //nome da fila
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments: null);

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var user = JsonConvert.DeserializeObject<User>(contentString);

                //NotifyUser(message);

                if (user != null)
                    await _userRepository.Create(user);

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };
        }
    }
}
