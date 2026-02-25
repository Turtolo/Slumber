using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Slumber;

public class DevStage : Stage
{    
    public DevStage() {}
    public Player Player;

    Vector2 Current = new Vector2(10, 10);

    public override void OnEnter()
    {
        base.OnEnter();

        var o = PathHelper.Combine("Raw", "LevelData", "Dev.json");

        Player = Engine.Tree.Create<Player>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(160, 40);
        });   

        Engine.Tree.Create<Camera2D>().SetProperties(n =>
        {
            n.LocalPosition = Player.GlobalPosition;
            n.SetParent(Player);
        }); 
 
        Engine.Tree.Create<StaticBody2D>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(0, 50);
            n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(100, 25);
            }));
        });

        var path = Engine.Tree.Create<Path2D>().SetProperties(n =>
        {
            n.SetPath(
                new Vector2(150, 50), 
                new Vector2(200, 50), 
                new Vector2(250, 100), 
                new Vector2(600, 100), 
                new Vector2(750, 200),
                new Vector2(750, 50),
                new Vector2(1500, 50)
            );
        });

        Engine.Tree.Create<Test>();

        Engine.Tree.Create<YBody>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(150, 70);
            n.Velocity.X = 50;
            n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(100, 25);
            }));
        });

        Engine.Tree.Create<Node2D>().SetProperties(n =>
        {
            n.AddChild(Engine.Tree.Create<ParallaxLayer>().SetProperties(n =>
            {
                n.Texture = new MTexture("Assets/Backgrounds/HeightsBGNoMain");
            }));

            n.AddChild(Engine.Tree.Create<ParallaxLayer>().SetProperties(n =>
            {
                n.Texture = new MTexture("Assets/Backgrounds/HeightsBG");
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

        foreach(var c in Engine.Tree.GetAll<PhysicsBody2D>())
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