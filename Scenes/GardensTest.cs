
using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class GardensTest : Node
{
  public Player Player;

  public PixelCamera Camera;

  public override void _EnterTree()
  {
    base._EnterTree();

    var root = new Node2D()
      .Set("Position", new Vector2(0, 60));

    Player = new Player()
      .Set("Position", new Vector2(200, -30));

    var bounds = new Rectangle(-320, -69, 717, 250);
    
    Camera = new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.TargetOffset = new Point(0, 65))
      .Set(n => n.Limit = bounds)
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

    Console.WriteLine(Camera.Transform.Global.Position);
  }

  public override void _Submit(Canvas2D canvas)
  {
    base._Submit(canvas);
  }
}
