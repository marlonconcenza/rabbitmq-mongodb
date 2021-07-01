using AutoMapper;
using Common.Models;
using Common.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConnectionFactory _factory;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _factory = new ConnectionFactory
            {
                HostName = Environment.GetEnvironmentVariable("Rabbitmq_Host")
            };

            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDTO dto)
        {
            var uuid = Guid.NewGuid();
            var queue = Environment.GetEnvironmentVariable("Rabbitmq_Queue");

            try
            {
                using (var connection = _factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: queue, //nome da fila
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        dto.uuid = uuid;

                        var stringfiedMessage = JsonConvert.SerializeObject(dto);

                        var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: queue, //nome da fila
                            basicProperties: null,
                            body: bytesMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok(uuid);
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> Get(string uuid)
        {
            try
            {
                Guid guid = Guid.Parse(uuid);

                var user = await _userRepository.Get(guid);

                if (user == null) return NotFound();

                var dto = _mapper.Map<UserDTO>(user);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var users = await _userRepository.Get();

                if (users == null) return NotFound();

                var dtos = new List<UserDTO>();

                foreach (var item in users)
                {
                    dtos.Add(_mapper.Map<UserDTO>(item));
                }

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
