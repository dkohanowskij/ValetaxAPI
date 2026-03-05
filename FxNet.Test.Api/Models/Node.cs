namespace FxNet.Test.Api.Models;

public class Node
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public long TreeId { get; set; }
    public long? ParentNodeId { get; set; }
    public Tree Tree { get; set; } = null!;
    public Node? ParentNode { get; set; }
    public ICollection<Node> Children { get; set; } = new List<Node>();
}
