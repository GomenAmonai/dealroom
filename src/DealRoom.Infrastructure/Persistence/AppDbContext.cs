using Microsoft.EntityFrameworkCore;
using DealRoom.Core.Entities;

namespace DealRoom.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Organization> Organizations { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Deal> Deals { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set;}
}
