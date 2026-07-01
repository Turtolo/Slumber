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

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);

    if (Core.Input.IsActionJustPressed("Jump"))
    {
      p.jumpBuffered = true;
      Await.Span(p.JumpBufferTime, () => p.jumpBuffered = false);
    }
  }

  public override void Update(float delta)
  {
    base.Update(delta);

    p.Sprite.PlayAnimation("Fall");

    if (p.IsOnFloor)
      Transition?.Invoke("IdleState");
  }
}
