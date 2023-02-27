using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class UsersController: BaseApiController
    {
        
        private readonly IUserRepository _services;
        private readonly IMapper _mapper;

        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository services, IMapper mapper, IPhotoService photo)
        {
            _mapper = mapper;
            _services = services;
            _photoService = photo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        
            return Ok(await _services.GetMembersAsync());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDTO>> GetUsers(string username){
            
            return await _services.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){
           
            var user = await _services.GetUserByUsername(User.GetUsername());

            _mapper.Map(memberUpdateDTO, user);

            _services.Update(user);

            if(await _services.SaveAllAsync()) return NoContent();

            return BadRequest("Update Failed");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file){

            var user = await _services.GetUserByUsername(User.GetUsername());

            var result = await _photoService.AddPhotoAsync(file);

            if(result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo{
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId  
            };

            if(user.Photos.Count == 0){
                photo.IsMain = true;
            }

            user.Photos.Add(photo);

            if(await _services.SaveAllAsync()){
                return CreatedAtAction(nameof(GetUsers), new {username = user.UserName}, _mapper.Map<PhotoDTO>(photo));

            }
                
            
            return BadRequest("Problem uploading photo");
        }

        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> SetMainPhoto(int photoId){
            var user = await _services.GetUserByUsername(User.GetUsername());

            if(user is null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo is null) return NotFound();

            if(photo.IsMain) return BadRequest("Photo is already set as Main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain == true);
            if(currentMain is not null) currentMain.IsMain = false;
            photo.IsMain = true;

            if(await _services.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the Main Photo");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId){
            var user = await _services.GetUserByUsername(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if(photo is null) return NotFound();

            if(photo.IsMain) return BadRequest("Main Photo can't be deleted!!");

            if(!string.IsNullOrEmpty(photo.PublicId)){
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);

                if(result.Error is not null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if(await _services.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the photo");
        }

    }
}