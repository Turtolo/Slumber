namespace Slumber;

public class Camera: Camera2D
{
  public Node2D Target { get; set; }

  public override void _PhysicsUpdate(float delta)
  {
    base._PhysicsUpdate(delta);

    Position = Target.Transform.Global.Position;
  }
}
