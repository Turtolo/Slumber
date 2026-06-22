
using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class GardensTest : Node
{
  public Player Player;

  public override void _EnterTree()
  {
    base._EnterTree();

    var root = new Node2D()
      .Set("Position", new Vector2(0, 60));

    new Parallax2D().Set(n =>
    {
      n.Texture = Core.Resource.Load<MTexture>("Graphics/Background/Gardens-Layer-1");
      n.Depth = -8;
      n.MotionScale = new Vector2(0.5f, 0f);
      n.RepeatSize = new Extent(640, 0);
      n.SetParent(root);
    });

    new Parallax2D().Set(n =>
    {
      n.Texture = Core.Resource.Load<MTexture>("Graphics/Background/Gardens-Layer-2");
      n.Depth = -9;
      n.MotionScale = new Vector2(0.3f, 0f);
      n.RepeatSize = new Extent(640, 0);
      n.SetParent(root);
    });

    new Parallax2D().Set(n =>
    {
      n.Texture = Core.Resource.Load<MTexture>("Graphics/Background/Gardens-Layer-3");
      n.Depth = -10;
      n.MotionScale = new Vector2(0.1f, 0f);
      n.RepeatSize = new Extent(640, 0);
      n.SetParent(root);
    });

    Player = new Player()
      .Set("Position", new Vector2(200, -30));
    
    new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.TargetOffset = new Point(0, 65))
      .Set(n => n.Deadzone = new Extent(30, 0))
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.Target = Player);

    var loader = Loader.Default();
    var mapPath = Path.Combine(
        AppContext.BaseDirectory,
        "Content",
        "Maps",
        "Gardens",
        "gardens-test-1.tmx"
    );

    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = Color.Black;
      //n.AmbientColor = Color.Black;
    });


    DotTiledBridge.Load(mapPath, loader);
  }

  public override void _ExitTree()
  {
    base._ExitTree();
  }

  public override void _PhysicsUpdate(float delta)
  {
    base._PhysicsUpdate(delta);
  }

  public override void _Process(float delta)
  {
    base._Process(delta);
  }

  public override void _Submit(Canvas2D canvas)
  {
    base._Submit(canvas);
  }
}
