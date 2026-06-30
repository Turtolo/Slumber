namespace Slumber;

public class IdleState : State
{
  Point Axis; 

  Player p => Core.Token.Get<Player>();

  public override void OnEnter()
  {

  }

  public override void OnExit()
  {

  }

  public override void Update(float delta)
  {
    p.Sprite.PlayAnimation("Idle");
  }

  public override void PhysicsUpdate(float delta)
  {
    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");

    HandleDeceleration(delta);

    if (Axis.X != 0) 
      Transition?.Invoke("RunState");

    if (Core.Input.IsActionJustPressed("Jump"))
      Transition?.Invoke("JumpState");

    if (!p.IsOnFloor)
      Transition?.Invoke("FallState");
  }

  
  public void HandleDeceleration(float delta)
  {
    if (!p.AllowControl || p._isDashing)
      return;

    p.Velocity.X = p.PlayerAxis.X == 0 ? p.MoveToward(p.Velocity.X, 0, p.Deceleration * delta) : p.Velocity.X;
  }
}
