
using System.Collections.Generic;
using System.Linq;
using Amethyst.Runtime;

namespace Slumber;

public class DevStage : Node
{
  public DevStage() { }

  public Player Player;


  public override void _EnterTree()
  {
    base._EnterTree();

    Player = new Player()
      .Set("LocalPosition", new Vector2(160, 40));

    new Node2D()
      .Set("LocalPosition", new Vector2(60, 20))
      .Set("LocalRotation", 2f);

    new Camera2D()
      .Set("LocalPosition", new Vector2(0, 40))
      .SetParent(Player);

    new StaticBody2D().Set(n =>
    {
      n.AddChild(new CollisionShape2D().Set("Shape", new RectangleShape2D(500, 10)));
      n.LocalPosition = new Vector2(150, 50);
    });

    new SnowEmitter();

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
      new Enemy().Set("LocalPosition", new Vector2(player.Transform.Global.Position.X + 20, player.Transform.Global.Position.Y));
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
