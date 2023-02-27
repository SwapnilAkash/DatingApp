using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AccountServices:IAccountServices
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;

        public AccountServices(DataContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }
        public async Task<ActionResult<UserDto>> ResgisterUser(RegisterDTOs registerdto){

            using var hmac = new HMACSHA512();

            var user = new AppUser{
                UserName =registerdto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password)),
                PasswordSalt = hmac.Key
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                username = user.UserName,
                token = _tokenService.CreateToken(user)
            };
        }

        public async Task<bool> UserExists(string username){
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }

        public async Task<ActionResult<UserDto>> Login(LoginDTO loginDto){

            var user = await _context.Users.Include(u => u.Photos).SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if(user == null)
                return new UserDto{
                    username = null,
                    token = "username",
                    photoUrl = null
                };
            
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0; i<computedHash.Length; i++){
                if(computedHash[i] != user.PasswordHash[i])
                    return new UserDto{
                        username = null,
                        token = "password",
                        photoUrl = null
                    };
            }

            return new UserDto{
                username = user.UserName,
                token = _tokenService.CreateToken(user),
                photoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url
            };
        }

    }

}