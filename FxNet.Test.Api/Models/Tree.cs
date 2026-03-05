namespace FxNet.Test.Api.Models;

public class Tree
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public ICollection<Node> Nodes { get; set; } = new List<Node>();
}
