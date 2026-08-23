using System.Linq;

namespace Slumber;

public class HazardZone : Area2D
{
  public override void _Process(float delta)
  {
    base._Process(delta);
   
    if (GetAnyBody() is Player p)
    {
      Main.GameManager.HitHazard();
    }
  }
}
