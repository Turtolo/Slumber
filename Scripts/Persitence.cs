namespace Slumber;

public class Persistence : Object
{
  public Vector2 CurrentRespawnPoint { get; set; }
  public string CurrentRespawnScene { get; set; }

  public string CurrentBonfireId { get; set; }

  public Vector2 LastSafePoint { get; set; }
}
