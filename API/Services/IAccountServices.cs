using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Services
{
    public interface IAccountServices
    {
        Task<ActionResult<UserDto>> ResgisterUser(RegisterDTOs registerdto);
         Task<bool> UserExists(string username);
         Task<ActionResult<UserDto>> Login(LoginDTO loginDto);
    }
}