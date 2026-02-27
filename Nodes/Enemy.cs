using System.Diagnostics.CodeAnalysis;
using Gum.DataTypes.Variables;
using RenderingLibrary.Graphics;

namespace Slumber;

public class Enemy : KinematicBody2D
{
    public AnimatedSprite2D Sprite;
    public Area2D TakeDamageArea;

    public RayCast2D RayRight;
    public RayCast2D RayLeft;

    public MTexture MainTexture;
    public MTexture WhiteTexture => MainTexture.CreateWhiteTexture();

    public float Gravity = 1300f;
    public float TerminalVelocity = 1200f;

    public int Health = 10;

    public int Direction = 1;

    public bool CanTakeDamage = true;

    public Enemy() {}

    public override void OnEnter()
    {
        base.OnEnter();

        MainTexture = new MTexture("Assets/Animations/grassspidersheet");

        var animations = AsepriteLoader.LoadAnimations
        (
            MainTexture,
            PathHelper.Combine("Raw/Raw/GrassSpider.json")
        );

        Sprite = Engine.Tree.Create<AnimatedSprite2D>().SetProperties(n =>
        {
            n.SetParent(this);
            n.Atlas = animations;
            n.IsLooping = true;
            n.LocalPosition = new Vector2(8, 8);
        });

        Engine.Tree.Create<CollisionShape2D>().SetProperties(n =>
        {
            n.Shape = new RectangleShape2D(16, 16);
            n.SetParent(this);
        });

        TakeDamageArea = Engine.Tree.Create<Area2D>().SetProperties(n =>
        {
            n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
            {
                c.Shape = new RectangleShape2D(16, 16);
            }));
            n.SetParent(this);
        });

        RayRight = Engine.Tree.Create<RayCast2D>().SetProperties(n =>
        {
            n.SetParent(this);
            n.TargetPosition = new Vector2(20, 50);
            n.LocalPosition = new Vector2(40, 0);
        });

        RayLeft = Engine.Tree.Create<RayCast2D>().SetProperties(n =>
        {
            n.SetParent(this);
            n.TargetPosition = new Vector2(-20, 50);
            n.LocalPosition = new Vector2(-24, 0);
        });


        LocalDepth = 6;
    }

    public override void PhysicsUpdate(float delta)
    {
        ApplyGravity(delta);
        HandleDamage();
        Flip();
        Move(delta);

        base.PhysicsUpdate(delta);

        Sprite.PlayAnimation("run");
    }

    public override void ProcessUpdate(float delta)
    {
        base.ProcessUpdate(delta);
    }

    public override void SubmitCall()
    {
        base.SubmitCall();

        //RayRight.Ray.Draw();
        //RayLeft.Ray.Draw();

        Engine.Screen.Call(new FontDrawCall
        {
            Font = Engine.BitmapFont,
            Text = $"Health: {Health}",
            Position = new Vector2(GlobalPosition.X - 16, GlobalPosition.Y - 16)
        });
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public void Move(float delta)
    {
        Velocity.X *= Direction;

        if (Direction == 1 && !RayRight.IsColliding)
            Direction = -1;

        if (Direction == -1 && !RayLeft.IsColliding)
            Direction = 1;
    }

    public void HandleDamage()
    {
        if (TakeDamageArea.AreaEntered(out var area))
        {
            if (area is Area2D && area.GetParent() is Player && CanTakeDamage)
            {
                TakeDamage(2);
                Console.WriteLine("Yes");
            }
        }

        if (Health <= 0) 
            QueueFree();
    }

    public void Flip()
    {
        if (Direction == 1)
            Sprite.LocalSpriteEffects = SpriteEffects.None;
        else 
            Sprite.LocalSpriteEffects = SpriteEffects.FlipHorizontally;
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        CanTakeDamage = false;
        Engine.Timer.Wait(0.3f, () => CanTakeDamage = true);
    }
    
    public void ApplyGravity(float delta)
    {
        if (!IsOnFloor)
        {
            Velocity.Y = MathF.Min(
                Velocity.Y + Gravity * delta,
                TerminalVelocity
            );
        }
        else if (Velocity.Y > 0)
        {
            Velocity.Y = 0;
        }
    }
}