namespace Slumber;

public class PixelCamera : Camera2D
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
    
    int peek = Core.Input.GetAxis("MoveUp", "MoveDown");
    
    bool standingStill = Target.Velocity.LengthSquared() < 0.001f;

    Vector2 velocityDir = new Vector2(Target.Velocity.X, 0);
    if (velocityDir != Vector2.Zero)
        velocityDir.Normalize();

    var rawTargetPos = new Vector2(
        FollowX ? Target.Transform.Global.Position.X : Position.X,
        FollowY ? Target.Transform.Global.Position.Y : Position.Y
    );

    Point dir = velocityDir.ToPoint();

    Point targetOffset = TargetOffset * dir;

    if (standingStill)
      targetOffset.Y += TargetOffset.Y * peek;
       
    _currentOffset = OffsetSmoothing 
      ? MathE.Lerp(_currentOffset, targetOffset, Weight) 
      : targetOffset;

    Offset = _currentOffset.ToVector2();

    var selfPos = Position.ToPoint();
    var targetPos = rawTargetPos.ToPoint();

    var modifiedTargetPos = Smoothing
      ? MathE.Lerp(selfPos, targetPos, Weight)
      : targetPos;

    Position = modifiedTargetPos.ToVector2();
  }
} 
