using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController:BaseApiController
    {
        private readonly IAccountServices _services;
        public AccountController(IAccountServices services)
        {
            _services = services;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> ResgisterUser(RegisterDTOs registerdto)
        {
             if(await _services.UserExists(registerdto.Username))
                return BadRequest("Username already exists!!");

            return await _services.ResgisterUser(registerdto);
            
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto){

            var result = _services.Login(loginDto);

            if(result.Result.Value.username is null && result.Result.Value.token == "username")
                return Unauthorized("Invalid username");
            
            if(result.Result.Value.username is null && result.Result.Value.token == "password")
                return Unauthorized("Invalid password");

            return await result;

        }

       
        
    }
}