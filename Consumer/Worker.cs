using AutoMapper;
using Common.Models;
using Common.Repositories;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
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
            var factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("Rabbitmq_Host")
            };

            try
            {
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

                _channel.BasicQos(0, 1, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
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

                        await _userRepository.Create(user);

                        _channel.BasicAck(eventArgs.DeliveryTag, false);
                    }
                };

                _channel.BasicConsume(Environment.GetEnvironmentVariable("Rabbitmq_Queue"), false, consumer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
        
        public override void Dispose()
        {
            _channel.Close();
            _connection.Close();

            base.Dispose();
        }
    }
}
