namespace Slumber;

public class RunState : State
{
  Player p => Core.Token.Get<Player>();
  Point Axis;

  public override void OnEnter()
  {

  }

  public override void OnExit()
  {

  }

  public override void Update(float delta)
  {
    p.Sprite.PlayAnimation("Run");
  }

  public override void PhysicsUpdate(float delta)
  {
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
}
