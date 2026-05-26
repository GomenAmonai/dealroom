using DealRoom.Core.Entities;
using System.Threading.Tasks;

namespace DealRoom.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User> CreateAsync(User user);
}