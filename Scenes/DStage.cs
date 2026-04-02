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


        //Map.LoadMap("Content/Maps/Stage1/map.tmx");

        
 
        Engine.Tree.Create<Player>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(130, 50);
        });

        Engine.Tree.Create<StaticBody2D>().SetProperties(n =>
        {
            n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(200, 20);
            }));
            n.LocalPosition = new Vector2(100, 80);
        });

        Engine.Tree.Create<StaticBody2D>().SetProperties(n =>
        {
            n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(200, 20);
            }));
            n.LocalPosition = new Vector2(300, 130);
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

        foreach (var c in Engine.Tree.GetAll<CollisionShape2D>())
        {
            Engine.Canvas.Call(new TextureDrawCall
            {
                Texture = Engine.Pixel,
                Scale = new Vector2(c.Shape.Size.Width, c.Shape.Size.Height),
                Color = Color.Blue,
                Position = c.GlobalPosition
            });
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}