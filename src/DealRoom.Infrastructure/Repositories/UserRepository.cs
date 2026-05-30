using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;
using DealRoom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace DealRoom.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
   private readonly AppDbContext _context;
   public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}