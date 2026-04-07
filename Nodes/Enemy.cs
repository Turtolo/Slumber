namespace Slumber;

public class Enemy : KinematicBody2D
{
    public AnimatedSprite2D Sprite;
    public Area2D TakeDamageArea;
    public ParticleEmitter2D Emitter;

    public float Speed;
    public float TargetSpeed;

    public RayCast2D RayRight;
    public RayCast2D RayLeft;

    public MTexture MainTexture;

    public float Gravity = 1300f;
    public float TerminalVelocity = 1200f;

    public int Health = 4;

    public int Direction = 1;

    public bool CanTakeDamage = true;

    public Enemy() {}

    public override void OnEnter()
    {
        base.OnEnter();

        MainTexture = new MTexture("Graphics/Atlas/grassspidersheet");

        var animations = AsepriteLoader.LoadAnimations
        (
            MainTexture,
            PathTools.Combine("Raw/Raw/GrassSpider.json")
        );

        Sprite = Engine.Tree.Create<AnimatedSprite2D>().SetProperties(n =>
        {
            n.SetParent(this);
            n.Atlas = animations;
            n.IsLooping = true;
            n.LocalPosition = new Vector2(8, 8);
            n.LocalShader = Engine.Resource.Load<Effect>("Graphics/Shader/WhiteEffect").Clone();
        });

        Engine.Tree.Create<CollisionShape2D>().SetProperties(n =>
        {
            n.Shape = new RectangleShape2D(16, 16);
            n.SetParent(this);
        });

        Emitter = Engine.Tree.Create<ParticleEmitter2D>().SetProperties(n =>
        {
            n.Properties = EmitterProperties.Identity with
            {
                EmitCount = 0,
                AngleVariance = 180f,
                ParticleProperties = ParticleProperties.Identity with
                {
                    ColorStart = Color.DarkSlateBlue,
                    ColorEnd = Color.MediumPurple
                }
            };
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

        SetNewTargetSpeed();

        LocalDepth = 6;
    }

    private void SetNewTargetSpeed()
    {
        float random = MathE.RandomFloat(0f, 1f);
        TargetSpeed = float.Lerp(20f, 120f, random * random);

        if (MathE.Random.NextDouble() < 0.2)
            TargetSpeed = 0;

        Engine.Timer.Wait(
            TimeSpan.FromSeconds(MathE.RandomFloat(0.5f, 2f)),
            () => SetNewTargetSpeed()
        );
    }

    public override void PhysicsUpdate(float delta)
    {
        ApplyGravity(delta);
        Flip();

        Speed = MoveToward(Speed, TargetSpeed, 200f * delta);

        Move(delta);

        base.PhysicsUpdate(delta);

        Sprite.PlayAnimation("run");
        Sprite.LocalShader.Parameters["overlayColor"].SetValue(Color.White.ToVector4());
    }

    public override void ProcessUpdate(float delta)
    {
        base.ProcessUpdate(delta);

        HandleDamage();
    }

    public override void SubmitCall()
    {
        base.SubmitCall();

        Engine.Canvas.Call(new FontDrawCall
        {
            Font = Engine.BitmapFont,
            Text = $"{Health}",
            Position = new Vector2(GlobalPosition.X - 16, GlobalPosition.Y - 16)
        });
    }

    public void Move(float delta)
    {
        Velocity.X = Speed * Direction;

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
            }
        }

        if (Health <= 0) 
        {
            Emitter.Emit(10);
            Emitter.SetParent(null);

            Engine.Timer.Wait(TimeSpan.FromSeconds(0.5f), () => Emitter.QueueFree());

            QueueFree();
        }
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
        Sprite.LocalShader.Parameters["enabled"].SetValue(1);
        Engine.EngineTime.TimeScale = 0f;

        Engine.Timer.WaitUnscaled(TimeSpan.FromSeconds(0.15f), () =>
        {
            CanTakeDamage = true;
            Sprite.LocalShader.Parameters["enabled"].SetValue(0);
        });

        Engine.Timer.WaitFrames(2, () =>
        {
            Engine.EngineTime.TimeScale = 1f;
        });
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

    public float MoveToward(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
            return target;

        return current + MathF.Sign(target - current) * maxDelta;
    }
}