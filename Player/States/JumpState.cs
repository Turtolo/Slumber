namespace Slumber;

public class JumpState : BaseAirState
{
  Player p => Core.Token.Get<Player>();

  public override void OnEnter()
  {
    base.OnEnter();

    Jump();
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
      ScreenEffects?.Invoke("FallState");
  }

  public override void Physics(float delta)
  {
    base.Physics(delta);

    HandleJump();
  }

  public void Jump()
  {
    p.Velocity.Y = p.Properties.JumpForce;
    p.Properties.JumpReleased = false;
    p.Properties.CanCoyoteJump = false;
    p.Properties.JumpBuffered = false;
  }

  public void HandleJump()
  {
    if (!p.Properties.JumpReleased && !Core.Input.IsActionPressed("Jump") && p.Velocity.Y < 0)
    {
      p.Velocity.Y /= 2f;
      p.Properties.JumpReleased = true;
    }
  }
}
