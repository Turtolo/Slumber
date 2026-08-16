using System.Linq;

namespace Slumber;

public class SceneChange : Area2D
{
  public string SceneName;

  public override void Process(float delta)
  {
    if (GetAnyBody() is Player p)
    {
      p.QueueFree();
      Main.Transition.Change(SceneName, Transform.Global.Position);
    }
  }
}

