using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services
{
    public interface IUserRepository
    {
        void Update(AppUser user);
        Task<bool> SaveAllAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserById(int id);
        Task<AppUser> GetUserByUsername(string username);
        Task<MemberDTO> GetMemberAsync(string username);
        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
    }
}