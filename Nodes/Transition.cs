using System.Linq;

namespace Slumber;

public class Transition : Node
{
  public AnimatedSprite2D Player;

  public override void EnterTree()
  {
    var transAn = AsepriteLoader.LoadAnimations(
        Core.Resource.Load<MTexture>("Graphics/Transition"),
        PathTools.Combine("Raw/Raw/Transition.json") 
    );

    Player = Core.Token.Create<AnimatedSprite2D>().Set(n =>
    {
      n.Atlas = transAn;
      //n.Rounded = true;
      n.Seperated = true;
      n.Position = new Vector2(320, 180);
      n.Scale = new Vector2(640, 360);
      n.Depth = 20;
      n.SetParent(this);
    });

    Player.Detach();
  }

  public override void Process(float delta)
  {

  }

  public void Reload()
  {
    In();
    Await.Until(() => Player.IsFinished, () => 
    {
      Core.Token.Anchor.ReloadCurrentAnchor();
      Out();
    });
  }

  public void Change(string nName, Vector2 entrancePosition)
  {
    In();
    Await.Until(() => Player.IsFinished, () =>
    {
      Type t = null;
      
      t = Type.GetType(nName);

      if (t == null)
      {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          t = assembly.GetTypes().FirstOrDefault(x => x.Name == nName || x.FullName == nName);
          if (t != null) break; 
        }
      }

      if (t != null)
      {
        var instance = Activator.CreateInstance(t);

        if (instance is Opal.Tools.Anchor a)
        {
          Core.Token.Anchor.SetAnchor(a);
          if (a is Scene s)
          {
            s.EntrancePosition = entrancePosition;
          }
        }
        else
        {
          Console.WriteLine($"Error: {nName} is not a Node.");
        }
      }
      else
      {
        Console.WriteLine($"Error: Could not find type matching '{nName}' in any assembly.");
      }

      Out();
    });
  }

  public void Change(Anchor n)
  {
    In();
    Await.Until(() => Player.IsFinished, () =>
    {
      Core.Token.Anchor.SetAnchor(n);
      In();
    });

  }

  public void In()
  {
    Player.PlayAnimation("In");
  }

  public void Out()
  {
    Player.PlayAnimation("Out");
  }
}
