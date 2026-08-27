using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Slumber
{
  public partial class Player : KinematicBody2D
  {
    #region Components
    
    public Raycast2D GroundCheck;

    public AnimatedSprite2D Sprite;

    public Area2D AttackArea;

    public Area2D TakeDamageArea;

    public List<Sprite2D> HealthIcons = new();

    public PlayerProperties Properties = new();

    public PauseMenu PauseMenu;

    public StateMachine STM;

    #endregion

    public Player()
    {
      Main.GameManager.Player = this;
    }

    public override void EnterTree()
    {
      base.EnterTree();
      
      PauseMenu = new PauseMenu();

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
        n.Shape = new RayCastShape2D(new Vector2(0, 25));
        n.Position = new Vector2(0, 10);
      });

      new PointLight2D().Set(n =>
      {
        n.Texture = Core.Resource.Load<MTexture>("Graphics/light");
        n.Position = new Vector2(-90, -75);
        n.Scale = new Vector2(2);
        n.SetParent(this);
      });

      AddHealthIcons();

      var idleState = new IdleState();
      var runState = new RunState();
      var fallState = new FallState();
      var jumpState = new JumpState();
      var landingState = new LandingState();
      var wallSlideState = new WallSlideState();
      var floorAttackState = new FloorAttackState();
      var nothingState = new NothingState();
      var spikeDamageState = new SpikeDamageState();

      STM = new StateMachine().Set(n =>
      {
        n.AddChild(idleState);
        n.AddChild(runState);
        n.AddChild(fallState);
        n.AddChild(jumpState);
        n.AddChild(landingState);
        n.AddChild(wallSlideState);
        n.AddChild(floorAttackState);
        n.AddChild(nothingState);
        n.AddChild(spikeDamageState);
        n.Initial = idleState;
        n.SetParent(this);
      });

      AddMask(1);
    }

    public void AddHealthIcons()
    {
      foreach (var i in Core.Token.GetAll("Health"))
        i.QueueFree();

      float iconSize = 16;
      float spacing = 2f;

      Vector2 startPosition = new Vector2(8, 8);

      for (int i = 0; i < Main.GameManager.Persistence.MaxHealthPoints; i++)
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
          n.Frame = 1;
        });

        HealthIcons.Add(s);
      }
      
      for (int i = 0; i < Main.GameManager.Persistence.CurrentHealthPoints; i++)
      {
        var icon = HealthIcons[i];
        icon.Frame = 0;
      }
    }

    public override void PhysicsUpdate(float delta)
    {
      base.PhysicsUpdate(delta);

      Properties.PlayerAxis = Core.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp").ToVector2();
      Main.GameManager.Persistence.PlayerViewDirection = (int)Properties.PlayerAxis.X != 0 ? (int)Properties.PlayerAxis.X : Main.GameManager.Persistence.PlayerViewDirection;

      MoveAndSlide(delta);

      Sprite.Shader.Parameters["overlayColor"].SetValue(Color.White.ToVector4());
    }

    public override void Process(float delta)
    {
      base.Process(delta);

      FlipSprite();
      HandleDamage();

      if (Properties.AttackCounter == 2)
        Properties.AttackCounter = 0;
    }

    public override void Submit(Canvas2D canvas)
    {
      base.Submit(canvas);

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
  }
}
