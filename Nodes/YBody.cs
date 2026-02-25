namespace Slumber;

public class YBody : DynamicBody2D
{
    public override void ProcessUpdate(float delta)
    {
        base.ProcessUpdate(delta);

        Velocity.Y = Engine.Tree.Get<Player>().PlayerAxis.Y * 50;
    }
}