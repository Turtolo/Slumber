using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Green : Node
{
  public Player Player;

  public override void _EnterTree()
  {
    base._EnterTree();

    var root = new Node2D()
      .Set("Position", new Vector2(0, 35));

    Player = new Player()
      .Set("Position", new Vector2(-105, -30));

    new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.TargetOffset = new Point(0, 65))
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.FollowY = false)
      .Set(n => n.Target = Player);

    var loader = Loader.Default();
    var mapPath = Path.Combine(
        AppContext.BaseDirectory,
        "Content",
        "Maps",
        "Green",
        "Green.tmx"
    );

    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = Color.CornflowerBlue;
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
