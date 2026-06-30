using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Slumber
{
  public class Player : KinematicBody2D
  {
    #region Configuration

    public float MoveSpeed = 130f;
    public float Acceleration = 3500f;
    public float Deceleration = 2500f;

    public int Health = 5;

    public float BaseGravity = 950f;
    public float FallGravity = 1950f;
    public float CurrentTerminalVelocity;
    public float InitialTerminalVelocity = 300f;
    public float SecondaryTerminalVelocity = 800f;
    public float JumpForce = -350;

    public float WallSlideGravity = 20f;
    public float WallJumpHorizontalSpeed = 200f;
    public float WallJumpVerticalSpeed = 300f;

    public float DashVelocity = 300f;

    public TimeSpan CoyoteTime = TimeSpan.FromSeconds(0.6f);
    public TimeSpan JumpBufferTime = TimeSpan.FromSeconds(0.2f);
    public TimeSpan AttackBufferTime = TimeSpan.FromSeconds(0.08f);

    public bool AllowControl = true;

    #endregion

    #region State

    public Vector2 PlayerAxis;
    public int PlayerDirection = 1;

    public Raycast2D GroundCheck;

    public bool CanTakeDamage = true;

    public bool jumpReleased = false;
    public bool wallSlideTriggered = false;
    public bool jumpBuffered = false;
    public bool canCoyoteJump = false;
    public bool wasOnFloor = false;

    public int attackCounter;
    public bool attackBuffer;
    public bool isAttacking = false;

    public bool _isDashing = false;

    public float _prevY;

    #endregion

    #region Components

    public AnimatedSprite2D Sprite;

    public Area2D AttackArea;

    public Area2D TakeDamageArea;

    List<Sprite2D> healthIcons = new();

    #endregion

    #region Constructors

    MTexture tex;

    public Player() { }

    public override void _EnterTree()
    {
      base._EnterTree();

      var c = Core.Token.Create<CollisionShape2D>().Set(n =>
      {
        n.Shape = new RectangleShape2D(8, 20);
        n.Position = new Vector2(0, 4);
        n.SetParent(this);
      });

      tex = Core.Resource.Load<MTexture>("Graphics/lan");

      var animations = AsepriteLoader.LoadAnimations(
          Core.Resource.Load<MTexture>("Graphics/Atlas/PlayerAnimation"),
          PathTools.Combine("Raw/Raw/PlayerAnimation.json")
      );

      Sprite = Core.Token.Create<AnimatedSprite2D>().Set(n =>
      {
        n.SetParent(this);
        n.Atlas = animations;
        n.Position = new Vector2(6, 9);
        n.IsLooping = true;
        n.Rounded = true;
        n.Shader = Core.Resource.Load<Effect>("Graphics/Shader/WhiteEffect").Clone();
      });

      Depth = 5;

      AttackArea = Core.Token.Create<Area2D>().Set(n =>
      {
        n.AddChild(Core.Token.Create<CollisionShape2D>().Set(c =>
          {
            c.Shape = new CircleShape2D(32);
            c.Disabled = true;
          }));
        n.SetParent(this);
        n.Name = "AttackArea";
      });

      var tC = c.Clone().Set(n =>
      {
        n.Position = new Vector2(0, 4);
      });

      TakeDamageArea = Core.Token.Create<Area2D>().Set(n =>
      {
        n.AddChild(tC);
        n.SetParent(this);
      });

      GroundCheck = new Raycast2D().Set(n =>
      {
        n.SetParent(this);
        n.Shape = new RayCastShape2D(new Vector2(0, 50));
        n.Position = new Vector2(0, 0);
      });

      AddHealthIcons();
    }
    #endregion

    #region Update

    public void AddHealthIcons()
    {
      foreach (var i in Core.Token.GetAll("Health"))
        i.QueueFree();


      float iconSize = 16;
      float spacing = 2f;

      Vector2 startPosition = new Vector2(8, 8);

      for (int i = 0; i < Health; i++)
      {
        Vector2 offset = new Vector2(i * (iconSize + spacing), 0);

        var s = new Sprite2D().Set(n =>
        {
          n.Texture = Core.Resource.Load<MTexture>("Graphics/Atlas/HealthIconSheetSmall");
          n.HFrames = 2;
          n.Position = startPosition + offset;
          n.Depth = 99;
          n.Name = "Health";
          n.Seperated = true;
        });

        healthIcons.Add(s);
      }
    }

    public override void _PhysicsUpdate(float delta)
    {
      base._PhysicsUpdate(delta);

      PlayerAxis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp").ToVector2();
      PlayerDirection = (int)PlayerAxis.X != 0 ? (int)PlayerAxis.X : PlayerDirection;

      HandleCoyoteTime();
      HandleJump();
      HandleDash();
      HandleMovementInput();
      HandleWallSlide();
      HandleDeceleration(delta);
      HandleAttack();
      ApplyGravity(delta);

      MoveAndSlide(delta);

      Sprite.Shader.Parameters["overlayColor"].SetValue(Color.White.ToVector4());
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      AnimateSprite();
      FlipSprite();
      HandleDamage();

      if (attackCounter == 2)
        attackCounter = 0;
    }

    public override void _Submit(Canvas2D canvas)
    {
      base._Submit(canvas);

      var fps = Math.Round(Core.Time.FPS);

      Color color = Color.White;

      if (fps > 20)
        color = Color.Red;
      if (fps > 40)
        color = Color.Yellow;
      if (fps >= 60)
        color = Color.Green;

      Core.Canvas.SubmitUnLit(new FontDrawCall
      {
        Params = CanvasParams.Identity with
        {
          Color = color,
          Position = new Vector2(625, 8),
        },
        Depth = 99,
        Font = Core.Resources.BitmapFont,
        Text = fps.ToString()
      });

      var t = ObjectPool<TextureDrawCall>.Get();

      t.Texture = tex;


      t.Params = CanvasParams.Identity with
      {
        Scale = new Vector2(1, 1),
        Position = new Vector2(Transform.Global.Position.X - (tex.Bounds.Width / 2), Transform.Global.Position.Y - (tex.Bounds.Height / 2))
      };

      t.Key = BatchKey.Default with
      {
        Matrix = Core.Token.Get<Camera2D>()?.GetTransform()
      };

      //canvas.SubmitLight(t);
    }

    #endregion

    #region Movement

    public TimeSpan DashDuration = TimeSpan.FromSeconds(0.2f);
    public TimeSpan DashCooldown = TimeSpan.FromSeconds(0.1f);

    public bool _canDash = true;

    public void HandleDash()
    {
      if (!_canDash)
        return;

      if (Core.Input.IsActionJustPressed("Dash"))
        Dash();
    }

    public void Dash()
    {
      _isDashing = true;

      Velocity.X = DashVelocity * PlayerDirection;
      Velocity.Y = 0;

      Await.Span(DashDuration, () =>
      {
        _isDashing = false;
        _canDash = false;
        Await.Span(DashCooldown, () => _canDash = true);
      });
    }

    public void HandleMovementInput()
    {
      if (!AllowControl || _isDashing)
        return;

      float targetSpeed = MoveSpeed * PlayerAxis.X;

      if (targetSpeed != 0)
        Velocity.X = MoveToward(Velocity.X, targetSpeed, Acceleration);
    }

    public void HandleDeceleration(float delta)
    {
      if (!AllowControl || _isDashing)
        return;

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
      if (_prevY > 0 && IsOnFloor)
        Land();

      _prevY = Velocity.Y;

      if (_isDashing)
        return;
      
      if (!IsOnFloor)
      {
        float activeGravity = Velocity.Y < 0 ? BaseGravity : FallGravity;
        
        if (GroundCheck.IsColliding())
          CurrentTerminalVelocity = InitialTerminalVelocity;
        else if (CurrentTerminalVelocity != SecondaryTerminalVelocity)
          CurrentTerminalVelocity = MathHelper.Lerp(CurrentTerminalVelocity, SecondaryTerminalVelocity, 0.1f);

        Velocity.Y = MathF.Min(
            Velocity.Y + activeGravity * delta,
            CurrentTerminalVelocity
        );
      }
      else if (Velocity.Y > 0)
      {
        Velocity.Y = 0;
      }
    }

    public void Land()
    {
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

    public void HandleCoyoteTime()
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

    public void AnimateSprite()
    {
      if (!isAttacking)
      {
        if (_isDashing)
        {
          Sprite.PlayAnimation("Dash");
        }
        else
        {
          if (IsOnFloor)
          {
            if (PlayerAxis.X != 0 && !IsOnWall)
              Sprite.PlayAnimation("Run");
            else
              Sprite.PlayAnimation("Idle");
          }
          else
          {
            Sprite.PlayAnimation("Fall");
          }
        }
      }
      else
      {
        if (PlayerAxis.X != 0)
          Sprite.PlayAnimation("RunAttack");
        else
          Sprite.PlayAnimation("Attack");
      }
    }

    public void FlipSprite()
    {
      if (PlayerDirection > 0)
      {
        Sprite.SpriteEffects = SpriteEffects.None;
        AttackArea.Position = new Vector2(40, 5);
      }
      else if (PlayerDirection < 0)
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
