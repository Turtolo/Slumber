namespace Slumber;

public class FallState : BaseAirState
{
  Player p => Core.Token.Get<Player>();
  Point Axis;

  public override void OnEnter()
  {
    base.OnEnter();
  }

  public override void OnExit()
  {
    base.OnExit();
  }

  public override void Physics(float delta)
  {
    base.Physics(delta);

    if (Core.Input.IsActionJustPressed("Jump"))
    {
      if (p.Properties.CanCoyoteJump)
      {
        Transition?.Invoke("JumpState");
      }

      p.Properties.JumpBuffered = true;
      Await.Span(p.Properties.JumpBufferTime, () => p?.Properties.JumpBuffered = false);
    }
  }

  public override void Update(float delta)
  {
    base.Update(delta);

    p.Sprite.PlayAnimation("Fall");

    if (p.IsOnFloor)
      Transition?.Invoke("LandingState");

    if (p.CanWall())
      Transition?.Invoke("WallSlideState");
  }
}
