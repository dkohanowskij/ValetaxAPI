using FxNet.Test.Api.Data;
using FxNet.Test.Api.DTOs;
using FxNet.Test.Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Api.Services;

public class JournalService : IJournalService
{
    private readonly AppDbContext _db;

    public JournalService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MRange<MJournalInfo>> GetRangeAsync(int skip, int take, VJournalFilter? filter)
    {
        var query = _db.ExceptionJournals.AsQueryable();

        if (filter != null)
        {
            if (filter.From.HasValue)
                query = query.Where(e => e.CreatedAt >= filter.From.Value);
            if (filter.To.HasValue)
                query = query.Where(e => e.CreatedAt <= filter.To.Value);
            if (!string.IsNullOrEmpty(filter.Search))
                query = query.Where(e => e.Text != null && e.Text.Contains(filter.Search));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(e => new MJournalInfo
            {
                Id = e.Id,
                EventId = e.EventId,
                CreatedAt = e.CreatedAt,
                Text = e.Text
            })
            .ToListAsync();

        return new MRange<MJournalInfo>
        {
            Skip = skip,
            Count = total,
            Items = items
        };
    }

    public async Task<MJournal> GetSingleAsync(long id)
    {
        var entry = await _db.ExceptionJournals.FindAsync(id);
        if (entry == null)
            throw new SecureException($"Journal entry {id} not found.");

        return new MJournal
        {
            Id = entry.Id,
            EventId = entry.EventId,
            CreatedAt = entry.CreatedAt,
            Parameters = entry.Parameters,
            StackTrace = entry.StackTrace,
            Text = entry.Text
        };
    }
}
