namespace Slumber;

public class BaseAirState : State
{
  Player p => Core.Token.Get<Player>();

  public override void PhysicsUpdate(float delta)
  {
    ApplyGravity(delta);
  }

  public void ApplyGravity(float delta)
  {
    if (p._prevY > 0 && p.IsOnFloor)
      p.Land();

    p._prevY = p.Velocity.Y;

    if (p._isDashing)
      return;
    
    if (!p.IsOnFloor)
    {
      float activeGravity = p.Velocity.Y < 0 ? p.BaseGravity : p.FallGravity;
      
      if (p.GroundCheck.IsColliding())
        p.CurrentTerminalVelocity = p.InitialTerminalVelocity;
      else if (p.CurrentTerminalVelocity != p.SecondaryTerminalVelocity)
        p.CurrentTerminalVelocity = MathHelper.Lerp(p.CurrentTerminalVelocity, p.SecondaryTerminalVelocity, 0.1f);

      p.Velocity.Y = MathF.Min(
          p.Velocity.Y + activeGravity * delta,
          p.CurrentTerminalVelocity
      );
    }
    else if (p.Velocity.Y > 0)
    {
      p.Velocity.Y = 0;
    }
  }
}
