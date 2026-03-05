using FxNet.Test.Api.DTOs;
using FxNet.Test.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FxNet.Test.Api.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
public class TreeController : ControllerBase
{
    private readonly ITreeService _treeService;

    public TreeController(ITreeService treeService)
    {
        _treeService = treeService;
    }

    /// <summary>
    /// Returns your entire tree. If your tree doesn't exist it will be created automatically.
    /// </summary>
    [HttpPost("api.user.tree.get")]
    [ProducesResponseType(typeof(MNode), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<MNode>> GetTree([FromQuery] string treeName)
    {
        var result = await _treeService.GetTreeAsync(treeName);
        return Ok(result);
    }
}
