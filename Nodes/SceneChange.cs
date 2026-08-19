using System.Linq;

namespace Slumber;

public class SceneChange : Area2D
{
  public string SceneName;

  public bool Trigger;

  public override void Process(float delta)
  {
    if (GetAnyBody() is Player p)
    {
      if (Trigger && !Core.Input.Keyboard.WasKeyJustPressed(Keys.W))
        return;

      p.QueueFree();
      Main.Transition.Change(SceneName, Transform.Global.Position);
    }
  }
}

