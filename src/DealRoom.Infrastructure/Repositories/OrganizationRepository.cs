using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;
using DealRoom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DealRoom.Infrastructure.Repositories;

public class OrganizationRepository : IOrganizationRepository
{
    private readonly AppDbContext _context;

    public OrganizationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Organization> CreateAsync(Organization organization)
    {
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();
        return organization;
    }

    public async Task<Organization?> GetByIdAsync(int id)
    {
        return await _context.Organizations.FirstOrDefaultAsync(o => o.Id == id);
    }
}
