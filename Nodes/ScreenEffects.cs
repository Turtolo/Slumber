using System.Linq;

namespace Slumber;

public class ScreenEffects : Node
{
  public AnimatedSprite2D Transition;

  public override void EnterTree()
  {
    var transAn = AsepriteLoader.LoadAnimations(
        Core.Resource.Load<MTexture>("Graphics/Transition"),
        PathTools.Combine("Raw/Raw/Transition.json") 
    );

    Transition = Core.Token.Create<AnimatedSprite2D>().Set(n =>
    {
      n.Atlas = transAn;
      //n.Rounded = true;
      n.Seperated = true;
      n.Position = new Vector2(320, 180);
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
    Transition.PlayAnimation("Out");
  }
}
