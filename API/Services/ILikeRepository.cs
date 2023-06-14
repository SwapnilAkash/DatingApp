using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services
{
    public interface ILikeRepository
    {
        //find users
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId); 
        //Check if user has already been liked by other users  
        Task<AppUser> GetUserWithLikes(int userId); 
        //find list of users that current user has liked or the list of users liked by current user
        Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likeParams); 
    }
}