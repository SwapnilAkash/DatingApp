using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserServices: IUserServices
    {
        private readonly DataContext _context;

        public UserServices(DataContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
            
            return await _context.Users.ToListAsync();
        }

        public async Task<ActionResult<AppUser>> GetUsers(int id){
            
            return await _context.Users.FindAsync(id);
        }
    }
}