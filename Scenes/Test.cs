

using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Test : Scene
{

  public AnimatedSprite2D Transition;

  public override void EnterTree()
  {
    base.EnterTree();

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
    });
  }

  public override void ExitTree()
  {
    base.ExitTree();
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);
  }

  public override void Process(float delta)
  {
    base.Process(delta);

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.T))
      Transition.PlayAnimation("Out");
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }
}
