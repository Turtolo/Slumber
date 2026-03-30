using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MonoGameGum.GueDeriving;
using MonoTile;

namespace Slumber;

public class DStage : Stage
{
    public DStage() {}

    List<Point> Path;
    int[,] CSV;

    public override void OnEnter()
    {
        base.OnEnter();

        var c = TiledLoader.Extract("Content/Maps/Stage1/map.tmx").Layers.FirstOrDefault().Tiles;

        Map.LoadMap("Content/Maps/Stage1/map.tmx");

        CSV = CordHelper.BuildNavGrid(c.GetLength(1), c.GetLength(0), 16);
        Path = AStar.GetPath(CSV, new Point(3, 12), new Point(8, 10));

        Engine.Tree.Create<Player>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(130, 0);
        });

        Engine.Tree.Create<Camera2D>().SetProperties(n =>
        {
            n.SetParent(Engine.Tree.Get<Player>());
        });
    }

    public override void PhysicsUpdate(float deltaTime)
    {
        base.PhysicsUpdate(deltaTime);
    }

    public override void ProcessUpdate(float deltaTime)
    {
        base.ProcessUpdate(deltaTime);
    }

    public override void SubmitCall()
    {
        base.SubmitCall();

        var rects = Map.GetRectangles(CSV);

        foreach (var rect in rects)
        {
            Point xy = new(rect.x, rect.y);
            Point wh = new(rect.width, rect.height);

            xy = xy.ToWorldCords(16);
            wh = wh.ToWorldCords(16);

            Rectangle r = new(xy.X, xy.Y, wh.X, wh.Y);

            r.ToShape().Draw(Color.Blue, 1);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}