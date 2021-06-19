using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteMongoDB.Models;
using TesteMongoDB.Repositories;

namespace TesteMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/<UsersController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var users = await _repository.Get();

            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(usersDTO);
        }

        // GET api/<UsersController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _repository.Get(ObjectId.Parse(id));

            var dto = _mapper.Map<UserDTO>(user);

            return Ok(dto);
        }

        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            var user = await _repository.GetByName(name);

            var dto = _mapper.Map<UserDTO>(user);

            return Ok(dto);
        }

        // POST api/<UsersController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDTO dto)
        {
            User user = _mapper.Map<User>(dto);

            var id = await _repository.Create(user);

            dto.id = id.ToString();

            return Ok(dto);
        }

        // PUT api/<UsersController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody] User user)
        {
            bool result = await _repository.Update(ObjectId.Parse(id), user);

            return result ? Ok() : BadRequest();
        }

        // DELETE api/<UsersController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            bool result = await _repository.Delete(ObjectId.Parse(id));
            return result ? Ok() : BadRequest();
        }
    }
}
