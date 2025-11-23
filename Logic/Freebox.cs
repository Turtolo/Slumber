


namespace Slumber;

public class FreeBox : Node
{   
    Area2D FreeArea;
    
    public FreeBox(NodeConfig config) : base(config) {}

    public override void Load()
    {
        base.Load();

        FreeArea = new(new NodeConfig
        {
            Shape = this.Shape,
            Name = "FreeBoxArea",
            Root = this
        });
    }

    public override void Unload()
    {
        base.Unload();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (FreeArea.BodyEntered(out KinematicBody2D body) && body is Player)
        {
            body.QueeFree();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        base.Draw(spriteBatch);
    }

}
