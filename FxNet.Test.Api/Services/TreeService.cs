using FxNet.Test.Api.Data;
using FxNet.Test.Api.DTOs;
using FxNet.Test.Api.Exceptions;
using FxNet.Test.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FxNet.Test.Api.Services;

public class TreeService : ITreeService
{
    private readonly AppDbContext _db;

    public TreeService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<MNode> GetTreeAsync(string treeName)
    {
        var tree = await _db.Trees.FirstOrDefaultAsync(t => t.Name == treeName);
        if (tree == null)
        {
            tree = new Tree { Name = treeName };
            _db.Trees.Add(tree);
            await _db.SaveChangesAsync();
        }

        var nodes = await _db.Nodes
            .Where(n => n.TreeId == tree.Id)
            .ToListAsync();

        return BuildTree(nodes, null);
    }

    public async Task CreateNodeAsync(string treeName, long? parentNodeId, string nodeName)
    {
        var tree = await _db.Trees.FirstOrDefaultAsync(t => t.Name == treeName);
        if (tree == null)
        {
            tree = new Tree { Name = treeName };
            _db.Trees.Add(tree);
            await _db.SaveChangesAsync();
        }

        if (parentNodeId.HasValue)
        {
            var parent = await _db.Nodes.FindAsync(parentNodeId.Value);
            if (parent == null)
                throw new SecureException($"Parent node {parentNodeId} not found.");
            if (parent.TreeId != tree.Id)
                throw new SecureException("Parent node does not belong to the specified tree.");

            // Check sibling uniqueness
            var siblingExists = await _db.Nodes.AnyAsync(n => n.ParentNodeId == parentNodeId && n.Name == nodeName);
            if (siblingExists)
                throw new SecureException($"A node named '{nodeName}' already exists under the same parent.");
        }
        else
        {
            // Root node uniqueness
            var rootExists = await _db.Nodes.AnyAsync(n => n.TreeId == tree.Id && n.ParentNodeId == null && n.Name == nodeName);
            if (rootExists)
                throw new SecureException($"A root node named '{nodeName}' already exists in this tree.");
        }

        var node = new Node
        {
            Name = nodeName,
            TreeId = tree.Id,
            ParentNodeId = parentNodeId
        };
        _db.Nodes.Add(node);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteNodeAsync(long nodeId)
    {
        var node = await _db.Nodes.FindAsync(nodeId);
        if (node == null)
            throw new SecureException($"Node {nodeId} not found.");

        // Check if node has children
        var hasChildren = await _db.Nodes.AnyAsync(n => n.ParentNodeId == nodeId);
        if (hasChildren)
            throw new SecureException("You have to delete all children nodes first");

        _db.Nodes.Remove(node);
        await _db.SaveChangesAsync();
    }

    public async Task RenameNodeAsync(long nodeId, string newNodeName)
    {
        var node = await _db.Nodes.FindAsync(nodeId);
        if (node == null)
            throw new SecureException($"Node {nodeId} not found.");

        // Check sibling uniqueness
        if (node.ParentNodeId.HasValue)
        {
            var siblingExists = await _db.Nodes.AnyAsync(n => n.ParentNodeId == node.ParentNodeId && n.Name == newNodeName && n.Id != nodeId);
            if (siblingExists)
                throw new SecureException($"A sibling node named '{newNodeName}' already exists.");
        }
        else
        {
            var rootExists = await _db.Nodes.AnyAsync(n => n.TreeId == node.TreeId && n.ParentNodeId == null && n.Name == newNodeName && n.Id != nodeId);
            if (rootExists)
                throw new SecureException($"A root node named '{newNodeName}' already exists in this tree.");
        }

        node.Name = newNodeName;
        await _db.SaveChangesAsync();
    }

    private MNode BuildTree(List<Node> allNodes, long? parentId)
    {
        var children = allNodes.Where(n => n.ParentNodeId == parentId).ToList();
        if (parentId == null)
        {
            // Virtual root node containing top-level tree nodes
            return new MNode
            {
                Id = 0,
                Name = "root",
                Children = children.Select(c => MapNode(allNodes, c)).ToList()
            };
        }
        throw new InvalidOperationException("BuildTree called with a non-null parentId; use MapNode for recursive child building.");
    }

    private MNode MapNode(List<Node> allNodes, Node node)
    {
        return new MNode
        {
            Id = node.Id,
            Name = node.Name,
            Children = allNodes.Where(n => n.ParentNodeId == node.Id).Select(c => MapNode(allNodes, c)).ToList()
        };
    }
}
