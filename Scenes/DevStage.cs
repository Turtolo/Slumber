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

        Random rand = new Random();

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
            n.LocalPosition = new Vector2(50, 50);
            n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(1300, 25);
            }));
        });

        for (int i = 0; i < 10; i++)
        {
            Engine.Tree.Create<Enemy>().SetProperties(n =>
            {
                n.LocalPosition = new Vector2(100 + i * 10, 40);
                n.Velocity.X = rand.Next(i * 5, 101);
            });
        }
 
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

        foreach(var c in Engine.Tree.GetAll<StaticBody2D>())
        {
            if (c.CollisionShape == null)
                continue;

            if (c.CollisionShape.Disabled)
                c.CollisionShape.Shape.Draw(Color.Gray, 1);
            else 
                c.CollisionShape.Shape.Draw(Color.Blue, 1);
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    
}