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
    
    if (Axis.X == 0)
      ScreenEffects?.Invoke("IdleState"); 
    if (Core.Input.IsActionJustPressed("Jump"))
      ScreenEffects?.Invoke("JumpState");
    if (!p.IsOnFloor)
      ScreenEffects?.Invoke("FallState");
    if (p.CanWall())
      ScreenEffects?.Invoke("WallSlideState");
    if (Core.Input.IsActionJustPressed("Attack"))
      ScreenEffects?.Invoke("FloorAttackState");
  }

  public override void Physics(float delta)
  {
    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");

    p.HandleMovementInput();
    p.HandleCoyoteTime();
  }
}
