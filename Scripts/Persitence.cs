namespace Slumber;

public class Persistence : Object
{
  public Vector2 CurrentRespawnPoint { get; set; }
  public string CurrentRespawnScene { get; set; }

  public int CurrentHealthPoints { get; set; } = 5;
  public int MaxHealthPoints { get; set; } = 5;
  
  public int PlayerViewDirection { get; set; } = 1;

  public string CurrentBonfireId { get; set; }

  public Vector2 LastSafePoint { get; set; }
}
