using System.Linq;

namespace Slumber;

public class ScreenEffects : Node
{
  public AnimatedSprite2D Transition;
  public AnimatedSprite2D Mist;

  public override void EnterTree()
  {
    var transAn = AsepriteLoader.LoadAnimations(
        new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/Transition"), new Rectangle(0, 0, 28, 1)),
        PathTools.Combine("Raw/Raw/Transition.json") 
    );

    Transition = Core.Token.Create<AnimatedSprite2D>().Set(n =>
    {
      n.Atlas = transAn;
      //n.Rounded = true;
      n.Seperated = true;
      n.Scale = new Vector2(640, 360);
      n.Depth = 20;
      n.SetParent(this);
    });

    Transition.Detach();
  }

  public void In()
  {
    Transition.PlayAnimation("In");
  }

  public void Out()
  {
    Console.WriteLine("Yes");
    Transition.PlayAnimation("Out");
  }
}
