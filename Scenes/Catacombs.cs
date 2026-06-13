
using System.Linq;
using MonoTile;

namespace Slumber;

public class Catacombs : Node
{
  public Player Player;

  public override void _EnterTree()
  {
    base._EnterTree();

    Player = new Player()
      .Set("Position", new Vector2(100, 0));

    new Camera2D()
      .Set("Position", new Vector2(0, 40))
      .SetParent(Player);

    SPTiledLoader.Extract("Content/Maps/Catacombs/map.tmx").ToMaps().ToTMap();
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
