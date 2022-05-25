using AutoMapper;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Minibank.Core.Domains.Users;
using Minibank.Web.Controllers.Users.Dto;
using Minibank.Core.Domains.Users.Services;

namespace Minibank.Web.Controllers.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task Create(UserDto userDto, CancellationToken cancellationToken = default)
        {
           await _userService.CreateAsync(_mapper.Map<User>(userDto), cancellationToken);
        }

        [HttpPut("{id}")]
        public async Task Update(int id, UserDto userDto, CancellationToken cancellationToken = default)
        {
            userDto.Id = id;
            await _userService.UpdateAsync(_mapper.Map<User>(userDto), cancellationToken);
        }

        [HttpDelete("{id}")]
        public async Task Delete(int id, CancellationToken cancellationToken = default)
        {
            await _userService.DeleteAsync(id, cancellationToken);
        }

        [HttpGet]
        public async Task<List<UserDto>> GetAll(CancellationToken cancellationToken = default)
        {
            return _mapper.Map<List<UserDto>>(await _userService.GetAllAsync(cancellationToken));
        }

        [HttpGet("{id}")]
        public async Task<UserDto> GetById(int id, CancellationToken cancellationToken = default)
        {
            return _mapper.Map<UserDto>(await _userService.GetByIdAsync(id, cancellationToken));
        }
    }
}