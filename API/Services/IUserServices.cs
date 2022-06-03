using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Services
{
    public interface IUserServices
    {
        Task<ActionResult<IEnumerable<AppUser>>> GetUsers();

        Task<ActionResult<AppUser>> GetUsers(int id);
    }
}