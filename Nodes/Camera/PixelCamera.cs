using System;
using Opal;
using Opal.Geometry;
using Opal.Hierarchy;
using Opal.Params;
using Opal.Tools;
using Microsoft.Xna.Framework;

namespace Slumber;

public class PixelCamera : Camera2D
{
  private Point _axis;

  private Vector2 _currentOffset;

  private Vector2 _setAsideOffset;

  private Vector2 _setAsidePosition;

  private bool yLocked;

  private Vector2 _cameraPlayerTargetPos;
  
  public bool toggleShake;

  private float shakeStrength;

  [Export]
  public KinematicBody2D Target { get; set; }

  [Export]
  public bool FollowX { get; set; } = true;

  [Export]
  public bool FollowY { get; set; } = true;

  [Export]
  public bool Smoothing { get; set; }

  [Export]
  public bool OffsetSmoothing { get; set; }

  [Export]
  public float Weight { get; set; } = 0.1f;

  [Export]
  public float RandomStrength { get; set; } = 30.0f;

  [Export]
  public float ShakeFade { get; set; } = 5.0f;

  [Export]
  public Point TargetOffset { get; set; } = Point.Zero;

  [Export]
  public Extent Deadzone { get; set; } = new Extent(30, 16);

  [Export]
  public Rectangle? Limit { get; set; }

  public override void EnterTree()
  {
    base._EnterTree();
    if (Target != null)
    {
      _cameraPlayerTargetPos = Target.Transform.Global.Position;
      base.Position = _cameraPlayerTargetPos;
    }
  }

  private Vector2 randomOffset()
  {
    return new Vector2(MathE.RandomFloat(-shakeStrength, shakeStrength), MathE.RandomFloat(-shakeStrength, shakeStrength));
  }

  public void Shake(Func<bool> action, float randomStrength = 30f, float shakeFade = 5f)
  {
    RandomStrength = randomStrength;
    ShakeFade = shakeFade;
    toggleShake = true;
    
    Await.Until(action, () => toggleShake = false);
  }

  public void Shake(TimeSpan length, float randomStrength = 30f, float shakeFade = 5f)
  {
    RandomStrength = randomStrength;
    ShakeFade = shakeFade;
    toggleShake = true;
    
    Await.Span(length, () => toggleShake = false);
  }

  public override void PhysicsUpdate(float delta)
  {
    base._PhysicsUpdate(delta);

    if (Target == null)
      return;

    if (Core.Time.TimeScale != 0)
    {
		  if (toggleShake)
			  shakeStrength = RandomStrength;
    }
	  if (shakeStrength > 0)
		  shakeStrength = MathHelper.Lerp(shakeStrength, 0, ShakeFade * delta);
		
		Offset = randomOffset();

    Point axis = Core.Input.GetAxis("CamLeft", "CamRight", "CamUp", "CamDown");

    if ((Target.Velocity.X != 0 || Target.Velocity.Y != 0) && axis.Y != 0)
      yLocked = true;

    if (Target.Velocity.X == 0 && Target.Velocity.Y == 0 && axis.Y == 0)
      yLocked = false;

    _axis = new Point(axis.X, (!yLocked) ? axis.Y : 0);

    if (MathF.Abs(_axis.X) > MathF.Abs(_axis.Y))
      _axis.Y = 0;
    else
      _axis.X = 0;

    Point point = TargetOffset * _axis;

    _currentOffset = OffsetSmoothing
      ? Vector2.Lerp(_currentOffset, new Vector2(point.X, point.Y), Weight)
      : point.ToVector2();

    Vector2 position = Target.Transform.Global.Position;
    Vector2 vector = Vector2.Floor(_currentOffset);

    if (FollowX)
    {
      float x = position.X - _cameraPlayerTargetPos.X;
      if (MathF.Abs(x) > (float)Deadzone.Width)
      {
        _cameraPlayerTargetPos.X = position.X - (float)(MathF.Sign(x) * Deadzone.Width);
      }
    }
    if (FollowY)
    {
      float x2 = position.Y - _cameraPlayerTargetPos.Y;
      if (MathF.Abs(x2) > (float)Deadzone.Height)
      {
        _cameraPlayerTargetPos.Y = position.Y - (float)(MathF.Sign(x2) * Deadzone.Height);
      }
    }
    Vector2 offset = base.Offset;

    float x3 = vector.X - offset.X;
    if (MathF.Abs(x3) > (float)Deadzone.Width)
    {
      offset.X = vector.X - (float)(MathF.Sign(x3) * Deadzone.Width);
    }
    float x4 = vector.Y - offset.Y;
    if (MathF.Abs(x4) > (float)Deadzone.Height)
    {
      offset.Y = vector.Y - (float)(MathF.Sign(x4) * Deadzone.Height);
    }

    base.Offset = offset;

    Vector2 cameraPlayerTargetPos = _cameraPlayerTargetPos;

    Point start = base.Position.ToPoint();
    Point point2 = cameraPlayerTargetPos.ToPoint();

    _setAsidePosition = (Smoothing ? MathE.Lerp(start, point2, Weight) : point2).ToVector2();

    if (Limit.HasValue)
    {
      Rectangle bounds = Limit.Value;
      Rectangle screenView = GetWorldViewRectangle();

      Rectangle worldView = new Rectangle((int)_setAsidePosition.X, (int)_setAsidePosition.Y, screenView.Width, screenView.Height);

      Vector2 center = new Vector2(
          worldView.Width * 0.5f, 
          worldView.Height * 0.5f);
      
      Vector2 min = new Vector2(bounds.Left + center.X, bounds.Top + center.Y);
      Vector2 max = new Vector2(bounds.Right - center.X, bounds.Bottom - center.Y);

      if (max.X < min.X)
        max.X = min.X = bounds.Left + (bounds.Width * 0.5f);
      if (max.Y < min.Y)
        max.Y = min.Y = bounds.Top + (bounds.Height * 0.5f);

      _setAsidePosition = Vector2.Clamp(_setAsidePosition, min, max);
    }

    base.Position = Vector2.Floor(_setAsidePosition);
  }
}
