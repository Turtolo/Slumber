using System.Xml.Linq;
using Gum.Forms.Controls;

namespace Slumber;

public class DevStage : Stage
{    
    public DevStage() {}

    public Player Player;

    ParticleEmitter2D Emitter1;
    ParticleEmitter2D Emitter2;


    public override void OnEnter()
    {
        base.OnEnter();

        Map.LoadMap("Content/Maps/Stage1/map.tmx");

        Player = Engine.Tree.Create<Player>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(160, 40);
        });   

        Engine.Tree.Create<Camera2D>().SetProperties(n =>
        {
            n.LocalPosition = new Vector2(0, 40);
            n.SetParent(Player);
        }); 

        
    
        Emitter1 = Engine.Tree.Create<SnowEmitter>().SetProperties(n =>
        {
            
        });

        Emitter2 = Engine.Tree.Create<SnowEmitter>().SetProperties(n =>
        {
            
        });
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
            Engine.Tree.Create<Enemy>().SetProperties(n =>
            {
                n.LocalPosition = new Vector2(Engine.Tree.Get<Player>().LocalPosition.X + 20, Engine.Tree.Get<Player>().LocalPosition.Y);
            });
        }

       var c = Engine.Tree.Get<Camera2D>();

       Emitter1.Emit(new Vector2(MathE.Random.Next(c.Bounds.Left - 320, c.Bounds.Right - 320), c.Bounds.Top - 20), 3);
       Emitter2.Emit(new Vector2(MathE.Random.Next(c.Bounds.Right - 320, c.Bounds.Right + 320), c.Bounds.Top - 20), 3);
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