using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using MonoGameGum.GueDeriving;
using Monolith.Tools;
using MonoTile;

namespace Slumber;

public class DStage : Stage
{
    public DStage() {}

    public override void OnEnter()
    {
        base.OnEnter();


        Map.LoadMap("Content/Maps/Stage1/map.tmx");

        Engine.Tree.Get<Tilemap>().SetProperties(n =>
        {
            n.LocalPosition = Vector2.Zero;
        });

 
        Engine.Tree.Create<Player>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(0, -50);
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
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}