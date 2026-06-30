using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Gardens : Node
{
  public Player Player;

  StateMachine machine;

  public override void _Ready()
  {

    //machine.ChangeState("teststate");
  }

  public override void _EnterTree()
  {
    base._EnterTree();

    var root = new Node2D()
      .Set("Position", new Vector2(0, 35));

    Player = new Player()
      .Set("Position", new Vector2(-50, 68));

    var rect = new Rectangle(-864, -96, 1152, 288);

    new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.TargetOffset = new Point(0, 65))
      .Set(n => n.Limit = rect)
      .Set(n => n.Deadzone = new Extent(30, 0))
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.Target = Player);


    var loader = Loader.Default();
    var mapPath = Path.Combine(
        AppContext.BaseDirectory,
        "Content",
        "Maps",
        "Gardens",
        "gardens-1.tmx"
    );
    
    var state = new TestState();

    machine = new StateMachine().Set(n =>
    {
      n.AddChild(state);
      n.Initial = state;
    });

    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = new Color(42, 63, 71);
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
  }
}
