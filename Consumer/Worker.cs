using AutoMapper;
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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private IConnection _connection;
        private IModel _channel;

        public Worker(ILogger<Worker> logger, IUserRepository userRepository, IMapper mapper)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mapper = mapper;

            InitRabbitMQ();
        }

        private void InitRabbitMQ()
        {
            _logger.LogInformation($"Iniciando Consumer");

            //Thread.Sleep(10000);

            //_logger.LogInformation($"Iniciando Consumer agora / {DateTime.Now.ToString("hh:mm:ss")}");

            var factory = new ConnectionFactory 
            { 
                HostName = Environment.GetEnvironmentVariable("Rabbitmq_Host") 
            };

            // create connection  
            _connection = factory.CreateConnection();

            // create channel  
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(
                            queue: Environment.GetEnvironmentVariable("Rabbitmq_Queue"), //nome da fila
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

            //_channel.ExchangeDeclare("demo.exchange", ExchangeType.Topic);
            //_channel.QueueBind("demo.queue.log", "demo.exchange", "demo.queue.*", null);
            //_channel.BasicQos(0, 1, false);

            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) =>
            {
                // received message  
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var dto = JsonConvert.DeserializeObject<UserDTO>(contentString);

                _logger.LogInformation($"Datetime: {DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} / JSON: {contentString}");

                if (dto != null)
                {
                    var user = _mapper.Map<User>(dto);

                    //NotifyUser(message);

                    await _userRepository.Create(user);
                }

                _channel.BasicAck(eventArgs.DeliveryTag, false);
            };

            consumer.Shutdown += OnConsumerShutdown;
            consumer.Registered += OnConsumerRegistered;
            consumer.Unregistered += OnConsumerUnregistered;
            consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

            _channel.BasicConsume(Environment.GetEnvironmentVariable("Rabbitmq_Queue"), false, consumer);
        }

        private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e) { }
        private void OnConsumerUnregistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerRegistered(object sender, ConsumerEventArgs e) { }
        private void OnConsumerShutdown(object sender, ShutdownEventArgs e) { }
        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e) { }

        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();
            base.Dispose();
        }
    }
}
