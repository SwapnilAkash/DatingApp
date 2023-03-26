using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class UserRepositoryServices : IUserRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public UserRepositoryServices(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<MemberDTO> GetMemberAsync(string username)
        {
            return await _context.Users.Where(x => x.UserName.Equals(username)).
            ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
        }

        public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
        {
            var query = _context.Users.AsQueryable();
            query = query.Where(u => !(u.UserName.Equals(userParams.currentUser)) && u.Gender.ToLower().Equals(userParams.Gender.ToLower()));

            var minDOB = DateTime.Today.AddYears(-userParams.maxAge - 1);
            var maxDOB = DateTime.Today.AddYears(-userParams.minAge);

            query = query.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);

            query = userParams.OrderBy switch {
                "created" => query.OrderByDescending(u => u.Created),
                _ => query.OrderByDescending(u => u.LastActive)

            };
            
            return await PagedList<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider),userParams.PageNumber,userParams.PageSize);
        }

        public async Task<AppUser> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser> GetUserByUsername(string username)
        {
            return await _context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<AppUser>> GetUsersAsync()
        {
            return await _context.Users.Include(p=>p.Photos).ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public void Update(AppUser user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }
    }
}