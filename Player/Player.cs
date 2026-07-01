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

    public PlayerFunctions Functions;

    public AnimatedSprite2D Sprite;

    public Area2D AttackArea;

    public Area2D TakeDamageArea;

    public List<Sprite2D> healthIcons = new();

    #endregion

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

      Functions = new(this);

      var idleState = new IdleState();
      var runState = new RunState();
      var fallState = new FallState();
      var jumpState = new JumpState();

      new StateMachine().Set(n =>
      {
        n.AddChild(idleState);
        n.AddChild(runState);
        n.AddChild(fallState);
        n.AddChild(jumpState);
        n.Initial = idleState;
        n.SetParent(this);
      });
    }

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

      //HandleCoyoteTime();
      //HandleJump();
      //HandleDash();
      //HandleMovementInput();
      //HandleWallSlide();
      //HandleDeceleration(delta);
      //HandleAttack();
      //ApplyGravity(delta);

      MoveAndSlide(delta);

      Sprite.Shader.Parameters["overlayColor"].SetValue(Color.White.ToVector4());
    }

    public override void _Process(float delta)
    {
      base._Process(delta);

      //AnimateSprite();
      Functions.FlipSprite();
      Functions.HandleDamage();

      //Console.WriteLine($"[Current]: {Get<StateMachine>()?.Current}");


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

      //canvas.SubmitLight(t);
    }

    public TimeSpan DashDuration = TimeSpan.FromSeconds(0.2f);
    public TimeSpan DashCooldown = TimeSpan.FromSeconds(0.1f);

    public bool _canDash = true;
  }
}
