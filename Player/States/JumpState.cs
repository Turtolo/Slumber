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
    
    p.Sprite.PlayAnimation("Fall");

    if (p.Velocity.Y >= 0)
      Transition?.Invoke("FallState");
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
    if (!p.jumpReleased && Core.Input.IsActionJustReleased("Jump") && p.Velocity.Y < 0)
    {
      p.Velocity.Y /= 2f;
      p.jumpReleased = true;
    }
  }
}
