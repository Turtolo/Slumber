

using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Gardens7 : Scene
{
  public Player Player;

  public override void EnterTree()
  {
    base.EnterTree();

    var root = new Node2D()
      .Set("Position", new Vector2(0, -150));
    
    var playerPos = new Vector2(168, -240);
    var playerDir = 1;

    Player = new Player();

    var rect = new Rectangle(-704, -440, 896, 360);

    new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.Limit = rect)
      .Set(n => n.Deadzone = new Extent(30, 0))
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.Smoothing = true)
      .Set(n => n.Target = Player);

    new Parallax2D().Set(n =>
    {
      n.Texture = new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/Background/Gardens-Layer-1"), new Rectangle(0, 0, 640, 360));
      n.Depth = -8;
      n.MotionScale = new Vector2(0.2f, 0f);
      n.RepeatSize = new Extent(640, 0);
      n.RepeatTimes = 4;
      n.SetParent(root);
    });

    new Parallax2D().Set(n =>
    {
      n.Texture = new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/Background/Gardens-Layer-2"), new Rectangle(0, 0, 640, 360));
      n.Depth = -9;
      n.MotionScale = new Vector2(0.3f, 0f);
      n.RepeatTimes = 4;
      n.RepeatSize = new Extent(640, 0);
      n.SetParent(root);
    });

    new Parallax2D().Set(n =>
    {
      n.Texture = new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/Background/Gardens-Layer-3"), new Rectangle(0, 0, 640, 609));
      n.Depth = -10;
      n.MotionScale = new Vector2(0.4f, 0f);
      n.RepeatTimes = 4;
      n.RepeatSize = new Extent(640, 0);
      n.Position = new Vector2(0, -125);
      n.SetParent(root);
    });

    var loader = Loader.Default();
    var mapPath = Path.Combine(
        AppContext.BaseDirectory,
        "Content",
        "Maps",
        "Gardens",
        "gardens-7.tmx"
    );
    
    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = new Color(13, 22, 24);
      n.AmbientColor = Color.White;
    });
    
    var t = DotTiledBridge.Load(mapPath, loader);
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

  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }
}
