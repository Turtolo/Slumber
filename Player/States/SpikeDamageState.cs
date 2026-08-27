
using System.Linq;

namespace Slumber;

public class SpikeDamageState : State
{
  Player p => Core.Token.Get<Player>();
  Point Axis;

  public override void OnEnter()
  {
    Core.Token.Get<PixelCamera>().Shake(TimeSpan.FromSeconds(0.05), 15, 10);

    p.Visible = false;
    p.Properties.CanTakeDamage = false;

    Main.GameManager.Persistence.CurrentHealthPoints -= 1;

    p.HealthIcons.LastOrDefault().Frame = 1;
    p.HealthIcons.RemoveAt(p.HealthIcons.Count - 1);

    p.Properties.AllowControl = false;

    Await.Until(() => p?.Position == Main.GameManager.Persistence.LastSafePoint, () =>
    {
      p.Properties.AllowControl = true;
      p.Properties.CanTakeDamage = true;
      p.Visible = true;
      Transition?.Invoke("IdleState");
    });
  }

  public override void OnExit()
  {

  }

  public override void Update(float delta)
  {
  }

  public override void Physics(float delta)
  {
  }
}
