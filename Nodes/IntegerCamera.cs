namespace Slumber;

public class IntegerCamera : Camera2D
{
  [Export]
  public Node2D Target { get; set; }

  public IntegerCamera() { }

  public override void _Process(float delta)
  {
    base._Process(delta);

    LocalPosition = Vector2.Round(Target.Transform.Global.Position);
  }
}
