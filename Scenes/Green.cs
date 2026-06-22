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
      .Set(n => n.Deadzone = new Extent(30, 0))
      .Set(n => n.OffsetSmoothing = true)
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
    
    new ColorRect().Set(n =>
    {
      n.Color = Color.White;
      n.Scale = new Vector2(100, 100);
      n.Depth = 99;
      n.Position = new Vector2(50, 0);
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

  Vector2 offset = new Vector2(20, 0);

  public override void _Process(float delta)
  {
    base._Process(delta);

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Y))
      new Enemy().Set("Position", Core.Token.Get<Player>()?.Transform.Global.Position + offset);

  }

  public override void _Submit(Canvas2D canvas)
  {
    base._Submit(canvas);

    return;

    TextureDrawCall call = ObjectPool<TextureDrawCall>.Get();
      
    call.Texture = Core.Resources.Pixel;
    call.Depth = 99;

    call.Params = CanvasParams.Identity with
    {
      Position = new Vector2(0, 0),
      Color = Color.Blue,
      Rotation = 0f,
      Scale = new Vector2(100, 100),
    };

    call.Key = BatchKey.Default with
    {
      Matrix = Core.Token.Get<Camera2D>()?.GetTransform(),
    };

    Core.Canvas.Submit(call);
  }
}
