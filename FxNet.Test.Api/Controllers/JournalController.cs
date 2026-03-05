using FxNet.Test.Api.DTOs;
using FxNet.Test.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FxNet.Test.Api.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
public class JournalController : ControllerBase
{
    private readonly IJournalService _journalService;

    public JournalController(IJournalService journalService)
    {
        _journalService = journalService;
    }

    /// <summary>
    /// Provides the pagination API. Skip means the number of items should be skipped by server. Take means the maximum number items should be returned by server. All fields of the filter are optional.
    /// </summary>
    [HttpPost("api.user.journal.getRange")]
    [ProducesResponseType(typeof(MRange<MJournalInfo>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<MRange<MJournalInfo>>> GetRange(
        [FromQuery] int skip,
        [FromQuery] int take,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] VJournalFilter? filter = null)
    {
        var result = await _journalService.GetRangeAsync(skip, take, filter);
        return Ok(result);
    }

    /// <summary>
    /// Returns the information about a particular event by ID.
    /// </summary>
    [HttpPost("api.user.journal.getSingle")]
    [ProducesResponseType(typeof(MJournal), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<MJournal>> GetSingle([FromQuery] long id)
    {
        var result = await _journalService.GetSingleAsync(id);
        return Ok(result);
    }
}
