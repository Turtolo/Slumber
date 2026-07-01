namespace Slumber;

public class BaseAirState : State
{
  Player p => Core.Token.Get<Player>();

  Point Axis; 

  public override void PhysicsUpdate(float delta)
  {
    Axis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");
    p.Functions.HandleMovementInput();
    p.Functions.HandleDeceleration(delta);
    p.Functions.ApplyGravity(delta);
  }
}
