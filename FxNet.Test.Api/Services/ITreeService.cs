using FxNet.Test.Api.DTOs;

namespace FxNet.Test.Api.Services;

public interface ITreeService
{
    Task<MNode> GetTreeAsync(string treeName);
    Task CreateNodeAsync(string treeName, long? parentNodeId, string nodeName);
    Task DeleteNodeAsync(long nodeId);
    Task RenameNodeAsync(long nodeId, string newNodeName);
}
