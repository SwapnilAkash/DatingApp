using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class LikeRepositoryService : ILikeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public LikeRepositoryService(DataContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }
        public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
        {
            return await _context.Likes.FindAsync(sourceUserId,targetUserId);
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.predicate.Equals("liked")){
                likes = likes.Where(x => x.SourceUserId == likesParams.userId);
                users = likes.Select(like => like.TargetUser);
            }

            if(likesParams.predicate.Equals("likedBy")){
                likes = likes.Where(x => x.TargetUserId == likesParams.userId);
                users = likes.Select(like => like.SourceUser);
            }

            return await PagedList<LikeDTO>.CreateAsync(users.ProjectTo<LikeDTO>(_mapper.ConfigurationProvider),likesParams.PageNumber,likesParams.PageSize);
        }

        public async Task<AppUser> GetUserWithLikes(int userId)
        {
            return await _context.Users.Include(x => x.LikedUsers).FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}