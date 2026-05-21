
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Amethyst.Runtime;
using DotTiled.Serialization;

namespace Slumber;

public class Heights : Node
{
  public Heights() { }

  public Player Player;

  int current = 1;

  public override void _EnterTree()
  {
    base._EnterTree();

    Player = new Player()
      .Set("Position", new Vector2(-100, -20));

    new IntegerCamera()
      .Set(n => n.Weight = 0.1f)
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.TargetOffset = new Point(40))
      .Set(n => n.Target = Player);

    new SnowEmitter();

    var loader = Loader.Default();
    DotTiledBridge.Load(Path.Combine(Core.Resource.ContentRoot, "Maps", "Heights", "Snow.tmx"), loader);
  }

  public override void _PhysicsUpdate(float deltaTime)
  {
    base._PhysicsUpdate(deltaTime);
  }

  public override void _Process(float deltaTime)
  {
    base._Process(deltaTime);

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.Y) || Core.Input.CurrentGamePad.WasButtonJustPressed(Buttons.LeftTrigger))
    {
      var player = Core.Index.Get<Player>();
      new Enemy().Set("Position", new Vector2(player.Transform.Global.Position.X + 20, player.Transform.Global.Position.Y));
    }

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.H) || Core.Input.CurrentGamePad.WasButtonJustPressed(Buttons.RightShoulder))
    {
      var player = Core.Index.Get<Player>();
      player.Health += 1;
      player.AddHealthIcons();
    }
  }

  public override void _SubmitCall()
  {
    base._SubmitCall();
  }

  public override void _ExitTree()
  {
    base._ExitTree();
  }

}
