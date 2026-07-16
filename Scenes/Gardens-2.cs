
using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Gardens2 : Node
{
  public Player Player;

  public override void EnterTree()
  {
    base.EnterTree();

    var root = new Node2D()
      .Set("Position", new Vector2(0, 35))
      .Set(n => n.SetParent(this));

    Player = new Player()
      .Set("Position", new Vector2(-176, 16))
      .Set(n => n.SetParent(this));

    var rect = new Rectangle(-272, -464, 528, 512);

    new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.TargetOffset = new Point(0, 65))
      .Set(n => n.FollowY = true)
      .Set(n => n.FollowX = false)
      .Set(n => n.Limit = rect)
      .Set(n => n.Deadzone = new Extent(30, 0))
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.Target = Player)
      .Set(n => n.SetParent(this));


    var loader = Loader.Default();
    var mapPath = Path.Combine(
        AppContext.BaseDirectory,
        "Content",
        "Maps",
        "Gardens",
        "gardens-2.tmx"
    );
    
    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = new Color(42, 63, 71);
      n.AmbientColor = Color.Gray;
      n.SetParent(this);
    });
    
    var t = DotTiledBridge.Load(mapPath, loader);

    foreach (var map in t)
      map.SetParent(this);
  }

  public override void ExitTree()
  {
    base.ExitTree();
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);
  }

  Vector2 offset = new Vector2(20, 0);

  public override void Process(float delta)
  {
    base.Process(delta);

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Y))
      new Enemy().Set("Position", Core.Token.Get<Player>()?.Transform.Global.Position + offset);

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.K))
      Core.Tree.SetScene(new Gardens());
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }
}
