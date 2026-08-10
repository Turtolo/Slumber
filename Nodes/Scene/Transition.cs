using System.Linq;

namespace Slumber;

public class Transition : Area2D
{
  public Scene CurrentScene { get; set; }
  public string TargetRoom { get; set; }

  public override void Process(float delta)
  {
    if (GetAnyBody() is Player p)
    {

    }
  }
}
