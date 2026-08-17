using System.Linq;

namespace Slumber;

public class KillZone : Area2D
{
  private bool _triggered;

  public override void _Process(float delta)
  {
    base._Process(delta);
    if (_triggered || CollisionShapes.IsEmpty()) return;
    
    if (GetAnyBody() is Player p)
    {
      _triggered = true;
      Core.Token.Get<PixelCamera>()?.toggleShake = true;
      p.QueueFree();
      Main.Transition.Reload();

      Await.Span(TimeSpan.FromSeconds(0.1f), () => Core.Token.Get<PixelCamera>()?.toggleShake = false);
    }
  }
}
