using System.Linq;

namespace Slumber;

public class SceneChange : Area2D
{
  public string TargetSceneName { get; set; }
  public string TargetGateID { get; set; }

  public bool Trigger;

  public override void Process(float delta)
  {
    if (GetAnyBody() is Player p)
    {
      if (Trigger && !Core.Input.IsActionJustPressed("Interact"))
        return;

      p.QueueFree();
      Main.GameManager.Change(TargetSceneName, TargetGateID);
    }
  }
}

