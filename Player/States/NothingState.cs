
namespace Slumber;

public class NothingState : State
{
  public override void OnEnter()
  {
    base.OnEnter();
  }

  public override void Update(float delta)
  {
    base.Update(delta);
  }

  public override void Physics(float delta)
  {
    base.PhysicsUpdate(delta);
  }
}
