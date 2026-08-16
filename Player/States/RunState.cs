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
      Transition?.Invoke("IdleState"); 
    if (Core.Input.IsActionJustPressed("Jump"))
      Transition?.Invoke("JumpState");
    if (!p.IsOnFloor)
      Transition?.Invoke("FallState");
    if (p.CanWall())
      Transition?.Invoke("WallSlideState");
  }

  public override void Physics(float delta)
  {
    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");

    p.HandleMovementInput();
    p.HandleCoyoteTime();
  }
}
