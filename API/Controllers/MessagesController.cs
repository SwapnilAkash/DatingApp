using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   public class MessagesController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IMessageRepository _messageRepository;
        public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
        {
            _messageRepository = messageRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO){

            var username = User.GetUsername();

            if(username.ToLower().Equals(createMessageDTO.RecipientUsername.ToLower()))
                return BadRequest("You cannot message yourself");
            
            var sender = await _userRepository.GetUserByUsername(username);
            var recipient = await _userRepository.GetUserByUsername(createMessageDTO.RecipientUsername);

            if(recipient is null) return NotFound();

            var message = new Message{
              Sender = sender,
              Recipient = recipient,
              Content = createMessageDTO.Content,
              SenderUsername = username,
              RecipientUsername = createMessageDTO.RecipientUsername  
            };

            _messageRepository.AddMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

            return BadRequest("Failed to send message");
        }
        
        [HttpGet]
        public async Task<ActionResult<PagedList<MessageDTO>>> GetMessageForUser([FromQuery]MessageParams messageParams){
            messageParams.Username = User.GetUsername();

            var messages = await _messageRepository.GetMessageForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage,messages.PageSize,messages.TotalPages,messages.TotalCount));

            return messages;
        }

        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username){
            var currentUsername = User.GetUsername();

            return Ok(await _messageRepository.GetMessageThread(currentUsername,username));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id){
            var username = User.GetUsername();
            var message = await _messageRepository.GetMessage(id);

            if(message.SenderUsername != username && message.RecipientUsername != username)
                return Unauthorized();
            
            if(message.SenderUsername == username) message.SenderDeleted = true;
            if(message.RecipientUsername == username) message.RecipientDeleted = true;

            if(message.SenderDeleted && message.RecipientDeleted)
                _messageRepository.DeleteMessage(message);

            if(await _messageRepository.SaveAllAsync()) return Ok();

            return BadRequest("Unable to delete the message");
            
        }
    }
}