using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class AccountServices:IAccountServices
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        
        private readonly IMapper _mapper;

        public AccountServices(DataContext context, ITokenService tokenService, IMapper mapper)
        {
            _context = context;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public async Task<ActionResult<UserDto>> ResgisterUser(RegisterDTOs registerdto){

            using var hmac = new HMACSHA512();

            var user = _mapper.Map<AppUser>(registerdto);
            user.UserName =registerdto.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerdto.Password));
            user.PasswordSalt = hmac.Key;
            

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new UserDto{
                username = user.UserName,
                token = _tokenService.CreateToken(user),
                knownAs =user.KnownAs,
                gender = user.Gender
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
                    photoUrl = null,
                    knownAs = null,
                    gender = null
                };
            
            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i=0; i<computedHash.Length; i++){
                if(computedHash[i] != user.PasswordHash[i])
                    return new UserDto{
                        username = null,
                        token = "password",
                        photoUrl = null,
                        knownAs = null,
                        gender = null
                    };
            }

            return new UserDto{
                username = user.UserName,
                token = _tokenService.CreateToken(user),
                photoUrl = user.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
                knownAs = user.KnownAs,
                gender = user.Gender
            };
        }

    }

}