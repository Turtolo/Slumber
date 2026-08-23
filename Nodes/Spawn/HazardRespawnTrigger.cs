namespace Slumber;

public class HazardRespawnTrigger : Area2D
{
  public override void Process(float delta)
  {
    if (GetAnyBody() is Player p)
    {
      var respawnPoint = new Vector2(Transform.Global.Position.X + (Get<CollisionShape2D>().Width / 2), Transform.Global.Position.Y);

      Main.GameManager.Persistence.LastSafePoint = respawnPoint;
    }
  }

}
