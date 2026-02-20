using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Slumber;

public class DevStage : Stage
{    
    public DevStage() {}

    public Player Player;

    public override void OnEnter()
    {
        base.OnEnter();

        var o = PathHelper.Combine("Raw", "LevelData", "Dev.json");

        Player = Engine.Node.Create<Player>().SetProperties(n =>
        {
            n.Position = new Vector2(160, 20);
        });   

        Engine.Node.Create<Camera2D>().SetProperties(n =>
        {
            n.Position = Player.Position;
            n.SetParent(Player);
        });

        Engine.Node.Create<StaticBody2D>().SetProperties(n =>
        {
            n.Position = new Vector2(0, 50);
            n.AddChild(Engine.Node.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(100, 25);
            }));
        });

        Engine.Node.Create<DynamicBody2D>().SetProperties(n =>
        {
            n.Position = new Vector2(150, 50);
            n.Velocity.X = 50;
            n.AddChild(Engine.Node.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(100, 25);
            }));
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

        foreach(var c in Engine.Node.GetAll<PhysicsBody2D>())
        {
            if (c is Player)
                continue;

            c.CollisionShape.Shape.Draw();
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    
}