using System.Collections.Generic;
using System.Linq;

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

  public Area2D LeftArea;
  public Area2D RightArea;

  public MTexture MainTexture;

  public float Gravity = 1300f;
  public float TerminalVelocity = 1200f;

  public int Health = 4;

  public int Direction = 1;

  public bool CanTakeDamage = true;

  public Enemy() { }

  public override void _EnterTree()
  {
    base._EnterTree();

    MainTexture = new MTexture("Graphics/Atlas/grassspidersheet");

    var animations = AsepriteLoader.LoadAnimations
    (
        MainTexture,
        PathTools.Combine("Raw/Raw/GrassSpider.json")
    );

    Sprite = Core.Index.Create<AnimatedSprite2D>().Set(n =>
    {
      n.SetParent(this);
      n.Atlas = animations;
      n.IsLooping = true;
      n.Position = new Vector2(8, 8);
      n.Shader = Core.Resource.Load<Effect>("Graphics/Shader/WhiteEffect").Clone();
    });

    var c = Core.Index.Create<CollisionShape2D>().Set(n =>
    {
      n.Shape = new RectangleShape2D(16, 16);
      n.SetParent(this);
    });

    Emitter = Core.Index.Create<ParticleEmitter2D>().Set(n =>
    {
      n.Params = EmitterParams.Identity with
      {
        EmitCount = 0,
        AngleVariance = 180f,
        Params = ParticleParams.Identity with
        {
          ColorStart = Color.DarkSlateBlue,
          ColorEnd = Color.MediumPurple
        }
      };
      n.SetParent(this);
    });

    TakeDamageArea = Core.Index.Create<Area2D>().Set(n =>
    {
      n.AddChild(Core.Index.Create<CollisionShape2D>().Set(c =>
        {
          c.Shape = new RectangleShape2D(16, 16);
        }));
      n.SetParent(this);
    });

    RayRight = Core.Index.Create<RayCast2D>().Set(n =>
    {
      n.SetParent(this);
      n.TargetPosition = new Vector2(20, 50);
      n.Position = new Vector2(40, 0);
    });

    RayLeft = Core.Index.Create<RayCast2D>().Set(n =>
    {
      n.SetParent(this);
      n.TargetPosition = new Vector2(-20, 50);
      n.Position = new Vector2(-24, 0);
    });

    var fullRect = new Rectangle(0, 0, 16, 16);

    var splitRects = fullRect.HorizontalSplit(2);

    LeftArea = new Area2D()
      .Set("Name", "LeftArea");

    LeftArea.AddChild(splitRects.First().ToCollisionShape());

    RightArea = new Area2D()
      .Set("Name", "RightArea");

    RightArea.AddChild(splitRects.Last().ToCollisionShape());

    LeftArea.SetParent(this);
    RightArea.SetParent(this);


    SetNewTargetSpeed();

    Depth = 6;
  }

  private void SetNewTargetSpeed()
  {
    float random = MathE.RandomFloat(0f, 1f);
    TargetSpeed = float.Lerp(20f, 120f, random * random);

    if (MathE.Random.NextDouble() < 0.2)
      TargetSpeed = 0;

    Await.Span(
        TimeSpan.FromSeconds(MathE.RandomFloat(0.5f, 2f)),
        () => SetNewTargetSpeed()
    );
  }

  public override void _PhysicsUpdate(float delta)
  {
    ApplyGravity(delta);
    Flip();

    Speed = MoveToward(Speed, TargetSpeed, 200f * delta);

    Move(delta);

    base._PhysicsUpdate(delta);

    Sprite.PlayAnimation("run");
    Sprite.Shader.Parameters["overlayColor"].SetValue(Color.White.ToVector4());
  }

  public override void _Process(float delta)
  {
    base._Process(delta);

    HandleDamage();
  }

  public override void _SubmitCall()
  {
    base._SubmitCall();
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
    var areas = TakeDamageArea.AreasEntered();
    if (areas.Any())
    {
      foreach (var area in areas)
      {
        if (area is Area2D && area.Name == "AttackArea" && CanTakeDamage)
        {
          TakeDamage(2);
        }
      }
    }

    if (Health <= 0)
    {
      Emitter.Emit(10);
      Emitter.SetParent(null);

      Await.Span(TimeSpan.FromSeconds(0.5f), () => Emitter.QueueFree());

      QueueFree();
    }
  }

  public void Flip()
  {
    if (Direction == 1)
      Sprite.SpriteEffects = SpriteEffects.None;
    else
      Sprite.SpriteEffects = SpriteEffects.FlipHorizontally;
  }

  public void TakeDamage(int amount)
  {
    Health -= amount;
    CanTakeDamage = false;
    Sprite.Shader.Parameters["enabled"].SetValue(1);


    Await.Span(TimeSpan.FromSeconds(0.15f), () =>
    {
      CanTakeDamage = true;
      Sprite.Shader.Parameters["enabled"].SetValue(0);
    });

    Core.Time.TimeScale = 0f;

    Await.Span(TimeSpan.FromSeconds(0.05f), () =>
    {
      Core.Time.TimeScale = 1f;
    }, true);
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
