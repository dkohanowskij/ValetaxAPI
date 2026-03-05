using FxNet.Test.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FxNet.Test.Api.Controllers;

[ApiController]
[Authorize]
[Produces("application/json")]
public class TreeNodeController : ControllerBase
{
    private readonly ITreeService _treeService;

    public TreeNodeController(ITreeService treeService)
    {
        _treeService = treeService;
    }

    /// <summary>
    /// Create a new node in your tree. You must specify a parent node ID that belongs to your tree or don't pass parent ID to create tree first level node. A new node name must be unique across all siblings.
    /// </summary>
    [HttpPost("api.user.tree.node.create")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> CreateNode(
        [FromQuery] string treeName,
        [FromQuery] long? parentNodeId,
        [FromQuery] string nodeName)
    {
        await _treeService.CreateNodeAsync(treeName, parentNodeId, nodeName);
        return Ok();
    }

    /// <summary>
    /// Delete an existing node. All children must be deleted first.
    /// </summary>
    [HttpPost("api.user.tree.node.delete")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> DeleteNode([FromQuery] long nodeId)
    {
        await _treeService.DeleteNodeAsync(nodeId);
        return Ok();
    }

    /// <summary>
    /// Rename an existing node in your tree. A new name of the node must be unique across all siblings.
    /// </summary>
    [HttpPost("api.user.tree.node.rename")]
    [ProducesResponseType(200)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RenameNode(
        [FromQuery] long nodeId,
        [FromQuery] string newNodeName)
    {
        await _treeService.RenameNodeAsync(nodeId, newNodeName);
        return Ok();
    }
}
