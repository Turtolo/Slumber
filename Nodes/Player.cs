using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Slumber
{
  public class Player : KinematicBody2D
  {
    #region Configuration

    public float MoveSpeed = 100f;
    public float Acceleration = 3500f;
    public float Deceleration = 2500f;

    public int Health = 5;

    public float Gravity = 1300f;
    public float TerminalVelocity = 1200f;
    public float JumpForce = -350;

    public float WallSlideGravity = 20f;
    public float WallJumpHorizontalSpeed = 200f;
    public float WallJumpVerticalSpeed = 300f;

    public TimeSpan CoyoteTime = TimeSpan.FromSeconds(0.6f);
    public TimeSpan JumpBufferTime = TimeSpan.FromSeconds(0.2f);
    public TimeSpan AttackBufferTime = TimeSpan.FromSeconds(0.2f);

    public bool AllowControl = true;

    #endregion

    #region State

    public Vector2 PlayerAxis;
    public int PlayerDirection;

    private bool CanTakeDamage = true;

    private bool jumpReleased = false;
    private bool wallSlideTriggered = false;
    private bool jumpBuffered = false;
    private bool canCoyoteJump = false;
    private bool wasOnFloor = false;

    private int attackCounter;
    private bool attackBuffer;
    private bool isAttacking = false;

    #endregion

    #region Components

    public AnimatedSprite2D Sprite;

    public Area2D AttackArea;

    public Area2D TakeDamageArea;

    List<Sprite2D> healthIcons = new();

    #endregion

    #region Constructors

    public Player() { }

    public override void _EnterTree()
    {
      base._EnterTree();

      var c = Core.Index.Create<CollisionShape2D>();
      c.Shape = new RectangleShape2D(10, 25);

      c.SetParent(this);

      var animations = AsepriteLoader.LoadAnimations(
          Core.Resource.Load<MTexture>("Graphics/Atlas/PlayerAnimation"),
          PathTools.Combine("Raw/Raw/PlayerAnimation.json")
      );

      Sprite = Core.Index.Create<AnimatedSprite2D>().Set(n =>
      {
        n.SetParent(this);
        n.Atlas = animations;
        n.Position = new Vector2(6, 9);
        n.IsLooping = true;
        n.Material.Local = n.Material.Local with { PixelPerfect = true };
        n.Shader = Core.Resource.Load<Effect>("Graphics/Shader/WhiteEffect").Clone();
      });

      Depth = 5;

      AttackArea = Core.Index.Create<Area2D>().Set(n =>
      {
        n.AddChild(Core.Index.Create<CollisionShape2D>().Set(c =>
          {
            c.Shape = new CircleShape2D(32);
            c.Disabled = true;
          }));
        n.SetParent(this);
        n.Name = "AttackArea";
      });

      var tC = c.Clone();

      TakeDamageArea = Core.Index.Create<Area2D>().Set(n =>
      {
        n.AddChild(tC);
        n.SetParent(this);
      });

      AddHealthIcons();
    }
    #endregion

    #region Update

    public void AddHealthIcons()
    {
      foreach (var i in Core.Index.GetAll("Health"))
        i.QueueFree();


      float iconSize = 16;
      float spacing = 2f;

      Vector2 startPosition = new Vector2(0, 0);

      for (int i = 0; i < Health; i++)
      {
        Vector2 offset = new Vector2(i * (iconSize + spacing), 0);

        var s = new Sprite2D().Set(n =>
        {
          n.Texture = Core.Resource.Load<MTexture>("Graphics/Atlas/HealthIconSheetSmall");
          n.HFrames = 2;
          n.Position = startPosition + offset;
          n.Depth = 50;
          n.Name = "Health";
          n.Visibility = false;
        });

        healthIcons.Add(s);
      }
    }

    public override void _PhysicsUpdate(float delta)
    {
      PlayerAxis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp").ToVector2();
      PlayerDirection = (int)PlayerAxis.X != 0 ? (int)PlayerAxis.X : PlayerDirection;

      HandleCoyoteTime();
      HandleJump();
      HandleMovementInput();
      HandleWallSlide();
      HandleDeceleration(delta);
      HandleAttack();
      ApplyGravity(delta);

      base._PhysicsUpdate(delta);

      Sprite.Shader.Parameters["overlayColor"].SetValue(Color.White.ToVector4());

      Position = new Vector2(MathF.Round(Position.X), Position.Y);
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      AnimateSprite();
      FlipSprite();
      HandleDamage();


      if (attackCounter == 2)
        attackCounter = 0;

      var rounded = new Vector2(
          MathF.Round(Position.X),
          MathF.Round(Position.Y)
      );

      var fractional = Position - rounded;

      Sprite.Position = new Vector2(
          6 - fractional.X,
          9 - fractional.Y
      );
    }

    public override void _SubmitCall()
    {
      base._SubmitCall();

      var fps = Math.Round(Core.FPS);

      Color color = Color.White;

      if (fps > 20)
        color = Color.Red;
      if (fps > 40)
        color = Color.Yellow;
      if (fps >= 60)
        color = Color.Green;

      Core.Canvas.Submit(new FontDrawCall
      {
        Params = CanvasParams.Identity with
        {
          Color = color,
          Position = new Vector2(6, 8),
        },
        Font = Core.BitmapFont,
        Text = fps.ToString()
      });
    }

    #endregion

    #region Movement

    public void HandleMovementInput()
    {
      if (!AllowControl)
        return;

      float targetSpeed = MoveSpeed * PlayerAxis.X;

      if (targetSpeed != 0)
        Velocity.X = MoveToward(Velocity.X, targetSpeed, Acceleration);
    }

    public void HandleDeceleration(float delta)
    {
      Velocity.X = PlayerAxis.X == 0 ? MoveToward(Velocity.X, 0, Deceleration * delta) : Velocity.X;
    }

    public float MoveToward(float current, float target, float maxDelta)
    {
      if (MathF.Abs(target - current) <= maxDelta)
        return target;

      return current + MathF.Sign(target - current) * maxDelta;
    }

    #endregion

    #region Jumping and Gravity

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

    public void HandleJump()
    {
      if (IsOnFloor || canCoyoteJump)
      {
        if (Core.Input.IsActionJustPressed("Jump") || jumpBuffered)
        {
          Velocity.Y = JumpForce;
          jumpReleased = false;
          canCoyoteJump = false;
          jumpBuffered = false;
        }
      }
      else
      {
        if (Core.Input.IsActionJustPressed("Jump"))
        {
          jumpBuffered = true;
          Await.Span(JumpBufferTime, () => jumpBuffered = false);
        }
      }

      if (!jumpReleased && Core.Input.IsActionJustReleased("Jump") && Velocity.Y < 0)
      {
        Velocity.Y /= 2f;
        jumpReleased = true;
      }
    }

    private void HandleCoyoteTime()
    {
      if (wasOnFloor && !IsOnFloor && Velocity.Y >= 0f)
      {
        canCoyoteJump = true;
        Await.Span(CoyoteTime, () => canCoyoteJump = false);
      }

      if (IsOnFloor)
        canCoyoteJump = false;

      wasOnFloor = IsOnFloor;
    }

    #endregion

    #region Wall Interaction

    public void HandleWallSlide()
    {
      if (PlayerAxis.X != 0 && IsOnWall && Velocity.Y > 0)
        wallSlideTriggered = true;

      if (!wallSlideTriggered)
        return;

      if (!IsOnWall || IsOnFloor)
        wallSlideTriggered = false;

      Velocity.Y = MathF.Min(
          Velocity.Y + WallSlideGravity,
          WallSlideGravity
      );

      if (Core.Input.IsActionJustPressed("Jump"))
        WallJump();
    }

    public void WallJump()
    {
      AllowControl = false;
      Await.Span(TimeSpan.FromSeconds(0.06f), () => AllowControl = true);

      if (PlayerDirection == 1)
        Velocity.X = -WallJumpHorizontalSpeed;
      else if (PlayerDirection == -1)
        Velocity.X = WallJumpHorizontalSpeed;

      Velocity.Y = -WallJumpVerticalSpeed;
    }

    #endregion

    #region Visuals

    private void AnimateSprite()
    {
      if (!isAttacking)
      {
        if (IsOnFloor)
        {
          if (PlayerAxis.X != 0)
            Sprite.PlayAnimation("Run");
          else
            Sprite.PlayAnimation("Idle");
        }
        else
        {
          Sprite.PlayAnimation("Fall");
        }
      }

      else
      {
        Sprite.PlayAnimation("Attack");
      }
    }

    private void FlipSprite()
    {
      if (PlayerAxis.X > 0)
      {
        Sprite.SpriteEffects = SpriteEffects.None;
        AttackArea.Position = new Vector2(40, 5);
      }
      else if (PlayerAxis.X < 0)
      {
        Sprite.SpriteEffects = SpriteEffects.FlipHorizontally;
        AttackArea.Position = new Vector2(-30, 5);
      }
    }

    #endregion

    #region Attack

    public void HandleDamage()
    {
      if (Health <= 0)
      {
        Core.Tree.ReloadCurrentScene();
      }

      if (!CanTakeDamage)
        return;

      var areas = TakeDamageArea.AreasEntered();

      if (areas.Any())
      {
        foreach (var area in areas)
        {
          if (area.Name == "LeftArea")
          {
            TakeDamage(1, -1);
            return;
          }

          if (area.Name == "RightArea")
          {
            TakeDamage(1, 1);
            return;
          }
        }
      }
    }

    public void TakeDamage(int damage, int dir)
    {
      Sprite.Shader.Parameters["enabled"].SetValue(1);
      CanTakeDamage = false;

      Health -= damage;

      healthIcons.LastOrDefault().Frame = 1;
      healthIcons.RemoveAt(healthIcons.Count - 1);

      AllowControl = false;

      Velocity = new Vector2(300 * dir, -300);

      Core.Time.TimeScale = 0f;

      Await.Span(TimeSpan.FromSeconds(0.2), () =>
      {
        Core.Time.TimeScale = 1f;
        AllowControl = true;
        Sprite.Shader.Parameters["enabled"].SetValue(0);
        CanTakeDamage = true;
      }, true);
    }

    public void HandleAttack()
    {
      if (Core.Input.IsActionJustPressed("Attack"))
      {
        if (!isAttacking)
        {
          Attack();
        }

        else
        {
          BufferAttack();
        }
      }

      if (isAttacking && Sprite.IsFinished)
      {
        AttackArea.Get<CollisionShape2D>().Disabled = true;
        isAttacking = false;

        if (attackBuffer)
        {
          attackBuffer = false;
          Attack();
        }
      }
    }

    public void Attack()
    {
      AttackArea.Get<CollisionShape2D>().Disabled = false;
      attackCounter++;
      isAttacking = true;
    }

    public void BufferAttack()
    {
      if (attackBuffer)
        return;

      attackBuffer = true;

      Await.Span(AttackBufferTime, () =>
      {
        attackBuffer = false;
      });
    }

    #endregion
  }
}
