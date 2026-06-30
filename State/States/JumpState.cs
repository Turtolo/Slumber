namespace Slumber;

public class JumpState : BaseAirState
{
  Player p => Core.Token.Get<Player>();

  public override void OnEnter()
  {
    Jump();

    base.OnEnter();
  }

  public override void OnExit()
  {
    base.OnExit();
  }

  public override void Update(float delta)
  {
    base.Update(delta);
    
    p.Sprite.PlayAnimation("Jump");

    if (p.Velocity <= 0)

  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);

    HandleJump();
  }

  public void Jump()
  {
    p.Velocity.Y = p.JumpForce;
    p.jumpReleased = false;
    p.canCoyoteJump = false;
    p.jumpBuffered = false;
  }

  public void HandleJump()
  {
    if (!p.IsOnFloor)
    {
      if (Core.Input.IsActionJustPressed("Jump"))
      {
        p.jumpBuffered = true;
        Await.Span(p.JumpBufferTime, () => p.jumpBuffered = false);
      }
    }

    if (!p.jumpReleased && Core.Input.IsActionJustReleased("Jump") && p.Velocity.Y < 0)
    {
      p.Velocity.Y /= 2f;
      p.jumpReleased = true;
    }
  }
}
