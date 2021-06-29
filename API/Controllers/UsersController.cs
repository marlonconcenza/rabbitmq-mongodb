using API.Options;
using Common.Models;
using Common.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConnectionFactory _factory;
        private readonly RabbitMqConfiguration _config;
        private readonly IUserRepository _userRepository;

        public UsersController(IOptions<RabbitMqConfiguration> options, IUserRepository userRepository)
        {
            _config = options.Value;

            _factory = new ConnectionFactory
            {
                HostName = _config.Host
            };

            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDTO dto)
        {
            var uuid = Guid.NewGuid();

            try
            {
                using (var connection = _factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(
                            queue: _config.Queue, //nome da fila
                            durable: false,
                            exclusive: false,
                            autoDelete: false,
                            arguments: null);

                        var user = new User
                        {
                            uuid = Guid.NewGuid(),
                            name = dto.name,
                            age = dto.age
                        };

                        var stringfiedMessage = JsonConvert.SerializeObject(user);

                        var bytesMessage = Encoding.UTF8.GetBytes(stringfiedMessage);

                        channel.BasicPublish(
                            exchange: "",
                            routingKey: _config.Queue, //nome da fila
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

        public async Task<IActionResult> Get(string uuid)
        {
            try
            {
                var user = await _userRepository.Get(Guid.Parse(uuid));

                if (user == null) return NotFound();

                var dto = new UserDTO
                {
                    name = user.name,
                    age = user.age
                };

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }

    public class UserDTO
    {
        public string name { get; set; }
        public int age { get; set; }
    }
}
