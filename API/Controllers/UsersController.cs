using API.DTOs;
using API.Entities;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        
        private readonly IUserRepository _services;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository services, IMapper mapper)
        {
            _mapper = mapper;
            _services = services;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        
            return Ok(await _services.GetMembersAsync());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUsers(string username){
            
            return await _services.GetMemberAsync(username);
        }

    }
}