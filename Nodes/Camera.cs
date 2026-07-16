namespace Slumber;

public class Camera: Camera2D
{
  public Node2D Target { get; set; }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);

    Position = Target.Transform.Global.Position;
  }
}
