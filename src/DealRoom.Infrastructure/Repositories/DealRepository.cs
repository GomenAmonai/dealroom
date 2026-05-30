using DealRoom.Core.Entities;
using DealRoom.Core.Interfaces;
using DealRoom.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DealRoom.Infrastructure.Repositories;

public class DealRepository : IDealRepository
{
    private readonly AppDbContext _context;

    public DealRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Deal?> GetByIdAsync(int id)
    {
        return await _context.Deals
            .Include(d => d.InitiatorOrganization)
            .Include(d => d.CounterpartyOrganization)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<IReadOnlyList<Deal>> GetByOrganizationAsync(int organizationId)
    {
        return await _context.Deals
            .Include(d => d.InitiatorOrganization)
            .Include(d => d.CounterpartyOrganization)
            .Where(d => d.InitiatorOrganizationId == organizationId
                     || d.CounterpartyOrganizationId == organizationId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<Deal> AddAsync(Deal deal)
    {
        _context.Deals.Add(deal);
        await _context.SaveChangesAsync();
        return deal;
    }

    public async Task UpdateAsync(Deal deal)
    {
        _context.Deals.Update(deal);
        await _context.SaveChangesAsync();
    }
}
