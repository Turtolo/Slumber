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

  [Export]
  public Extent DeadZone { get; set; }
  
  private Point _axis;

  private Point _currentOffset;

  private Vector2 _setAsideOffset;
  private Vector2 _setAsidePosition;

  public override void _Process(float delta)
  {
    base._Process(delta);

    //_axis = new Point(Core.Input.GetAxis("MoveLeft", "MoveRight"), Target.IsOnFloor ? Core.Input.GetAxis("MoveUp", "MoveDown") : 0);

    _axis = Target.IsOnFloor ? Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveUp", "MoveDown") : Point.Zero;

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
      ? MathE.Lerp(_currentOffset, targetOffset, Weight)
      : targetOffset;


    var rawTargetPos = new Vector2(
        FollowX ? Target.Transform.Global.Position.X : Position.X,
        FollowY ? Target.Transform.Global.Position.Y : Position.Y
    );

    var selfPos = Position.ToPoint();
    var targetPos = rawTargetPos.ToPoint();

    var modifiedTargetPos = Smoothing
      ? MathE.Lerp(selfPos, targetPos, Weight)
      : targetPos;

    _setAsideOffset = _currentOffset.ToVector2();
    _setAsidePosition = modifiedTargetPos.ToVector2();

    Offset = _setAsideOffset;
    Position = _setAsidePosition;
  }
} 
