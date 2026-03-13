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

        for (int i = 0; i < 20; i++)
        {
            Engine.Tree.Create<Enemy>().SetProperties(n =>
            {
                n.LocalPosition = new Vector2(100 + i * 10, 40);
                n.Speed = rand.Next(1, 100);
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

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.Y))
        {
            Engine.Tree.Create<Enemy>().SetProperties(n =>
            {
                n.Speed = MathM.Random.Next(60, 100);
                n.LocalPosition = new Vector2(Engine.Tree.Get<Player>().LocalPosition.X + 20, Engine.Tree.Get<Player>().LocalPosition.Y);
            });
        }
    }

    public override void SubmitCall()
    {
        base.SubmitCall();

        Engine.Canvas.Call(new FontDrawCall
        {
            Font = Engine.BitmapFont,
            Text = Engine.FPS.ToString()
        });

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