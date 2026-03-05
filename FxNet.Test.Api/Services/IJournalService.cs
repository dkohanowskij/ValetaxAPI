using FxNet.Test.Api.DTOs;

namespace FxNet.Test.Api.Services;

public interface IJournalService
{
    Task<MRange<MJournalInfo>> GetRangeAsync(int skip, int take, VJournalFilter? filter);
    Task<MJournal> GetSingleAsync(long id);
}
