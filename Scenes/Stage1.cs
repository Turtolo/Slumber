using System.Collections.Generic;
using System.IO;
using System.Linq;
using MonoTile;

namespace Slumber;

public class Stage1 : Stage
{  
    public override void OnEnter()
    {
        base.OnEnter();

        Map.LoadMap("Content/Maps/Stage2/map.tmx");

        Engine.Tree.Create<Player>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(160, 140);
        });   
 
        Engine.Tree.Create<Eagle>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(160, 80);
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

        //foreach (var c in Engine.Tree.GetAll<CollisionShape2D>())
            //c.Shape.Draw(Color.Blue, 1);
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}