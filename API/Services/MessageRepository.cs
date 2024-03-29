using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            this._context = context;
            
        }
        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDTO>> GetMessageForUser(MessageParams messageParams)
        {
            var query = _context.Messages.OrderByDescending(x => x.MessageSent).AsQueryable();

            query = messageParams.Container switch{
                "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && !x.RecipientDeleted),
                "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && !x.SenderDeleted),
                _ => query.Where(x => x.RecipientUsername == messageParams.Username && x.DateRead == null && !x.RecipientDeleted)  //Unread messages
            };

            var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDTO>.CreateAsync(messages,messageParams.PageNumber,messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(
                    x => x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername && !x.SenderDeleted 
                    || x.SenderUsername == recipientUsername && x.RecipientUsername == currentUsername && !x.RecipientDeleted
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();
            
            var unreadMessages =messages.Where(m => m.DateRead == null && m.RecipientUsername == currentUsername).ToList();

            if(unreadMessages.Any()){
                foreach(var message in unreadMessages){
                    message.DateRead = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDTO>>(messages);

        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}