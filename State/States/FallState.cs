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
    
    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");

    HandleMovementInput();
  }

  public void HandleMovementInput()
  {
    if (!p.AllowControl || p._isDashing)
      return;

    float targetSpeed = p.MoveSpeed * p.PlayerAxis.X;

    if (targetSpeed != 0)
      p.Velocity.X = p.MoveToward(p.Velocity.X, targetSpeed, p.Acceleration);
  }

  public override void Update(float delta)
  {
    base.Update(delta);

    p.Sprite.PlayAnimation("Fall");
  }
}
