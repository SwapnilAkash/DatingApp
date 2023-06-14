using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikeRepository _likeRepository;

        public LikesController(IUserRepository userRepository, ILikeRepository likeRepository)
        {
            this._userRepository = userRepository;
            this._likeRepository = likeRepository;
        }

        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username){
            
            var sourceUserId = User.GetUserId();
            var likedUser = await _userRepository.GetUserByUsername(username);
            var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

            if(likedUser is null) return NotFound();
            if(sourceUser.UserName.Equals(likedUser.UserName)) return BadRequest("You cannot like yourself");

            var userLike = new UserLike{
                SourceUserId = sourceUserId,
                TargetUserId = likedUser.Id
            };

            sourceUser.LikedUsers.Add(userLike);

            if(await _userRepository.SaveAllAsync())return Ok();

            return BadRequest("Failed to Like User");

        }

        [HttpGet]
        public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery] LikesParams likesParams){

            likesParams.userId = User.GetUserId();
            
            var users = await _likeRepository.GetUserLikes(likesParams);

            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,users.PageSize,users.TotalPages,users.TotalCount));

            return Ok(users);
        }
        
    }
}