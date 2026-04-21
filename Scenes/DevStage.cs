
using System.Collections.Generic;

namespace Slumber;

public class DevStage : Stage
{    
    public DevStage() {}

    public Player Player;

    public override void OnEnter()
    {
        base.OnEnter();

        Map.LoadMap("Content/Maps/Stage1/map.tmx");

        Player = Engine.Tree.Create<Player>().Set(n =>
        {
            n.LocalPosition = new Vector2(160, 40);
        });   

        Engine.Tree.Create<Camera2D>().Set(n =>
        {
            n.LocalPosition = new Vector2(0, 40);
            n.SetParent(Player);
        }); 

        Engine.Tree.Create<SnowEmitter>();
      }

    public override void PhysicsUpdate(float deltaTime)
    {
        base.PhysicsUpdate(deltaTime);
    }  

    public override void ProcessUpdate(float deltaTime)
    {
        base.ProcessUpdate(deltaTime);

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.Y))
        {
            Engine.Tree.Create<Enemy>().Set(n =>
            {
                n.LocalPosition = new Vector2(Engine.Tree.Get<Player>().LocalPosition.X + 20, Engine.Tree.Get<Player>().LocalPosition.Y);
            });
        }

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.G))
            Engine.Tree.Get<Enemy>().TakeDamage(1);

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.U))
        {
          Engine.Tree.Get<Player>().grod = true;
        }

    }

    public override void SubmitCall()
    {
        base.SubmitCall();

        foreach (var shape in Engine.Tree.GetAll<CollisionShape2D>())
        {
            if (shape.GetParent() is not DynamicBody2D || shape.GetParent().GetParent() is Tilemap)
                continue;
            
            Engine.Canvas.Call(new TextureDrawCall
            {
                Texture = Engine.Pixel,
                Params = CanvasParams.Identity with
                {
                  Scale = new Vector2(shape.Width, shape.Height),
                  Position = shape.Transform.Global.Position
                }
            });
        }
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    
}
