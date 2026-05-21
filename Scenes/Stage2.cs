using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Stage2 : Node
{
  public Player Player;

  public override void _EnterTree()
  {
    base._EnterTree();

    var ParRoot = new Node2D()
      .Set("Position", new Vector2(0, -280));

    new ParallaxLayer()
      .Set(n => n.Texture = Core.Resource.Load<MTexture>("Graphics/Background/Gardens-Layer-1"))
      .Set("Depth", -8)
      .Set("LoopAxes", LoopAxis.X)
      .SetParent(ParRoot);


    new ParallaxLayer()
      .Set(n => n.Texture = Core.Resource.Load<MTexture>("Graphics/Background/Gardens-Layer-2"))
      .Set("Depth", -9)
      .Set("LoopAxes", LoopAxis.X)
      .SetParent(ParRoot);


    new ParallaxLayer()
      .Set(n => n.Texture = Core.Resource.Load<MTexture>("Graphics/Background/Gardens-Layer-3"))
      .Set("Depth", -10)
      .Set("LoopAxes", LoopAxis.X)
      .SetParent(ParRoot);

    Player = new Player()
      .Set("Position", new Vector2(220, -100));

    new IntegerCamera()
      .Set(n => n.Weight = 1)
      .Set(n => n.Smoothing = true)
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.Target = Player);

    var loader = Loader.Default();
    DotTiledBridge.Load(Path.Combine(Core.Resource.ContentRoot, "Maps", "Test", "Test.tmx"), loader);
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

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Y) || Core.Input.CurrentGamePad.WasButtonJustPressed(Buttons.LeftTrigger))
    {
      var player = Core.Index.Get<Player>();
      new Enemy().Set("Position", new Vector2(player.Transform.Global.Position.X + 20, player.Transform.Global.Position.Y));
    }

    //foreach (var t in Core.Index.GetAll<Tilemap>())
    //Console.WriteLine($"Depth: {t.Ordering.Global.Depth}, Name: {t.Name}");
  }

  public override void _SubmitCall()
  {
    base._SubmitCall();
  }
}
