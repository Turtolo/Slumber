namespace Slumber;

public class BaseAirState : State
{
  Player p => Core.Token.Get<Player>();

  Point Axis; 

  public override void Physics(float delta)
  {
    base.Physics(delta);

    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");

    p.HandleMovementInput();
    p.HandleDeceleration(delta);
    p.ApplyGravity(delta);
  }
}
