using API.Options;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UsersController(IOptions<RabbitMqConfiguration> options, IUserRepository userRepository, IMapper mapper)
        {
            _config = options.Value;

            _factory = new ConnectionFactory
            {
                HostName = _config.Host
            };

            _userRepository = userRepository;
            _mapper = mapper;
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

                        dto.uuid = uuid;

                        var stringfiedMessage = JsonConvert.SerializeObject(dto);

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
