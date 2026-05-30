using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;
using DealRoom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DealRoom.Infrastructure.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly AppDbContext _context;

    public MessageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Message> AddAsync(Message message)
    {
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return message;
    }

    public async Task<IReadOnlyList<Message>> GetByDealAsync(int dealId)
    {
        return await _context.Messages
            .Where(m => m.DealId == dealId)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }
}
