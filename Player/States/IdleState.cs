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

    if (Axis.X != 0) 
      ScreenEffects?.Invoke("RunState");

    if (Core.Input.IsActionJustPressed("Jump") || p.Properties.JumpBuffered)
      ScreenEffects?.Invoke("JumpState");

    if (!p.IsOnFloor)
      ScreenEffects?.Invoke("FallState");

    if (Core.Input.IsActionJustPressed("Attack"))
      ScreenEffects?.Invoke("FloorAttackState");
  }

  public override void Physics(float delta)
  {
    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");

    p.HandleDeceleration(delta);
    p.HandleCoyoteTime();
  }
}
