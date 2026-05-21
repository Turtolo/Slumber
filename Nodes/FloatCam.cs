namespace Slumber;

public class FloatCamera : Camera2D
{
  [Export]
  public Player Target { get; set; }

  [Export]
  public bool FollowX { get; set; } = true;

  [Export]
  public bool FollowY { get; set; } = true;

  [Export]
  public bool Smoothing { get; set; } = false;

  [Export]
  public bool OffsetSmoothing { get; set; } = false;

  [Export]
  public float Weight { get; set; } = 0.1f;

  [Export]
  public Vector2 TargetOffset { get; set; } = Vector2.Zero;

  private Vector2 _axis;

  private Vector2 _currentOffset;

  public override void _Process(float delta)
  {
    base._Process(delta);

    _axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveUp", "MoveDown").ToVector2();

    if (MathF.Abs(_axis.X) > MathF.Abs(_axis.Y))
    {
      _axis.Y = 0;
    }
    else
    {
      _axis.X = 0;
    }

    var targetOffset = TargetOffset * _axis;

    _currentOffset = OffsetSmoothing
      ? Vector2.Lerp(_currentOffset, targetOffset, Weight)
      : targetOffset;

    Offset = Vector2.Round(_currentOffset);

    var targetPos = new Vector2(
        FollowX ? Target.Transform.Global.Position.X : Position.X,
        FollowY ? Target.Transform.Global.Position.Y : Position.Y
    );

    Position = Smoothing
      ? Vector2.Lerp(Position, targetPos, Weight)
      : targetPos;
  }
}
