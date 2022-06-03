using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController: ControllerBase
    {
        
        private readonly IUserServices _services;
        public UsersController(IUserServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            
            return await _services.GetUsers();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUsers(int id){
            
            return await _services.GetUsers(id);
        }

    }
}