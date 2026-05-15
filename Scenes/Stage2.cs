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

    Player = new Player()
      .Set("LocalPosition", new Vector2(220, -100));

    new Camera2D()
      .Set("LocalPosition", new Vector2(0, 40))
      .SetParent(Player);

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
      new Enemy().Set("LocalPosition", new Vector2(player.Transform.Global.Position.X + 20, player.Transform.Global.Position.Y));
    }

    //foreach (var t in Core.Index.GetAll<Tilemap>())
    //Console.WriteLine($"Depth: {t.Ordering.Global.Depth}, Name: {t.Name}");
  }

  public override void _SubmitCall()
  {
    base._SubmitCall();
  }
}
