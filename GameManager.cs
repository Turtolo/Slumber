using System.Linq;

namespace Slumber;

public class GameManager : Object
{
  public Persistence Persistence { get; set; } = new();  

  public Player Player { get; set; }

  public ScreenEffects ScreenEffectss;

  public GameManager()
  {
    ScreenEffectss = new ScreenEffects().Set(n => n.Detach());
  }

  public void Save(Checkpoint c)
  {
    Persistence.CurrentRespawnPoint = c.Transform.Global.Position;
    Persistence.CurrentRespawnScene = Core.Token.Anchor.GetCurrentAnchor().GetType().Name;

    FileT.ToBinary(Persistence, "Saved/Persistence");
  }

  public void Load()
  {
    FileT.FromBinary(Persistence, "Saved/Persistence");

    Transition(Persistence.CurrentRespawnScene, () =>
    {
      Player.Position = Persistence.CurrentRespawnPoint;
    });
  }

  public void Change(string targetScene, string targetID)
  {
    Transition(targetScene, () =>
    {
      var s = Core.Token.Anchor.GetCurrentAnchor() as Scene;
      s.EntranceGateID = targetID;
      Player.Position = s.SpawnPoints[targetID];
    });
  }

  public void Transition(string targetScene, Action onNewScene)
  {
    ScreenEffectss.In();
    Await.Until(() => ScreenEffectss.Transition.IsFinished, () =>
    {
      Type t = null;
      
      t = Type.GetType(targetScene);

      if (t == null)
      {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          t = assembly.GetTypes().FirstOrDefault(x => x.Name == targetScene || x.FullName == targetScene);
          if (t != null) break; 
        }
      }

      if (t != null)
      {
        var n = Core.Token.Anchor.SetAnchor(t);
        
        ScreenEffectss.Out();
      }
      else
      {
        Console.WriteLine($"Error: Could not find type matching '{targetScene}' in any assembly.");
      }

      ScreenEffectss.Out();
      Await.Until(() => Core.Token.Anchor.GetCurrentAnchor() != null, onNewScene);
    });
  }

  private bool canBeHazard = true;

  public void HitHazard()
  {
    if (!canBeHazard)
      return;

    canBeHazard = false;

    Player.SpikeDamage();

    ScreenEffectss.In();
    Await.Until(() => ScreenEffectss.Transition.IsFinished, () =>
    {
      Player.Position = Persistence.LastSafePoint;
      canBeHazard = true;
      ScreenEffectss.Out();
    });
  }

  public void TriggerDeath()
  {
    var cam = Core.Token.Get<PixelCamera>();
    cam?.toggleShake = true;
    Await.Span(TimeSpan.FromSeconds(0.1f), () => cam?.toggleShake = false);
    Player.QueueFree();

    Transition(Persistence.CurrentRespawnScene, () =>
    {
      Player.Position = Persistence.CurrentRespawnPoint;
    });

  }
}
