namespace Slumber;

public class State : Node
{
  public Action<string> Transition { get; set; }

  public virtual void OnEnter() {}

  public virtual void OnExit() {}

  public virtual void Update(float delta) {}

  public virtual void Physics(float delta) {}
}
