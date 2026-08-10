using System.Linq;

namespace Slumber;

public class KillZone : Area2D
{
  public override void _Process(float delta)
  {
    base._Process(delta);

    if (CollisionShapes.IsEmpty())
      return;
    
    if (GetAnyBody() is Player p)
    {
      Core.Token.Anchor.ReloadCurrentAnchor();
    }

  }
}
