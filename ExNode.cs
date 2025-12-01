public record class ExNodeConfig : Node2DConfig
{
    public RegionNode2D Region { get; set; }
}

public class ExampleNode : Node2D
{
    public ExampleNode(ExNodeConfig config) : base(config)
    {
        
    }
}