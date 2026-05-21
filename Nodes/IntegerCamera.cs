namespace Slumber;

public class IntegerCamera : Camera2D
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
  public Point TargetOffset { get; set; } = Point.Zero;

  private Point _axis;

  private Point _currentOffset;

  public override void _Process(float delta)
  {
    base._Process(delta);

    bool left = Core.Input.IsActionPressed("MoveLeft");
    bool right = Core.Input.IsActionPressed("MoveRight");
    bool up = Core.Input.IsActionPressed("MoveUp");
    bool down = Core.Input.IsActionPressed("MoveDown");

    if (_axis.X == 0 && _axis.Y == 0)
    {
      if (left)
        _axis.X = -1;
      else if (right)
        _axis.X = 1;
    }
    else if (_axis.X != 0)
    {
      if ((_axis.X == -1 && !left) ||
          (_axis.X == 1 && !right))
      {
        _axis.X = 0;
      }
    }

    if (_axis.Y == 0 && _axis.X == 0)
    {
      if (up)
        _axis.Y = -1;
      else if (down)
        _axis.Y = 1;
    }
    else if (_axis.Y != 0)
    {
      if ((_axis.Y == -1 && !up) ||
          (_axis.Y == 1 && !down))
      {
        _axis.Y = 0;
      }
    }

    var targetOffset = TargetOffset * _axis;

    _currentOffset = OffsetSmoothing
      ? MathE.Lerp(_currentOffset, targetOffset, Weight)
      : targetOffset;

    Offset = _currentOffset.ToVector2();

    var rawTargetPos = new Vector2(
        FollowX ? Target.Transform.Global.Position.X : Position.X,
        FollowY ? Target.Transform.Global.Position.Y : Position.Y
    );

    var selfPos = Position.ToPoint();
    var targetPos = rawTargetPos.ToPoint();

    var modifiedTargetPos = Smoothing
      ? MathE.Lerp(selfPos, targetPos, Weight)
      : targetPos;

    Position = modifiedTargetPos.ToVector2();

    Console.WriteLine(Offset);
  }
}
