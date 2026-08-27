

using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class PropTest
{
  public bool Alive = true;
  public int Health = 5;
  public float Pi = 3.14F;
  public string Name = "Alice";
  public Vector2 Pos = new Vector2(10, 20);
}

public class T2D : Node2D
{
  public Raycast2D GroundCheck;

  public AnimatedSprite2D Sprite;

  public Area2D AttackArea;

  public Area2D TakeDamageArea;

  public List<Sprite2D> HealthIcons = new();

  public PlayerProperties Properties = new();

  public PauseMenu PauseMenu;

  public StateMachine STM;

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

    //AddMask(1);
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
}

public class Test : Scene
{
  public PropTest Props;

  public Player Player;

  public List<Sprite2D> HealthIcons = new();

  public override void EnterTree()
  {
    base.EnterTree();

    Props = new PropTest();

    FileT.ToBinary(Props, "Saved/Test");

    Props.Alive = false;
    Props.Health = 4;
    Props.Pi = 3.2F;
    Props.Name = "Greg";
    Props.Pos = new Vector2(15, 25);
    
    Console.WriteLine("[BEFORE LOAD]");
    Console.WriteLine($"Alive: {Props.Alive}, Health: {Props.Health}, Pi: {Props.Pi}, Name: {Props.Name}, Pos: {Props.Pos.ToString()}");
    
    FileT.FromBinary(Props, "Saved/Test");
    Console.WriteLine("[AFTER LOAD]");
    Console.WriteLine($"Alive: {Props.Alive}, Health: {Props.Health}, Pi: {Props.Pi}, Name: {Props.Name}, Pos: {Props.Pos.ToString()}");

    new ColorRect().Set(n =>
    {
      n.Scale = new Vector2(64);
      n.Color = Color.Red;
    });

    //var p = new Plire().Set(n => n.Position = new Vector2(100, 50));
    var p = new Player();

    new PixelCamera()
      .Set(n => n.Weight = 0.3f)
      .Set(n => n.Deadzone = new Extent(30, 0))
      .Set(n => n.OffsetSmoothing = true)
      .Set(n => n.Smoothing = true)
      .Set(n => n.Target = p);

    new CanvasAnchor().Set(n =>
    {
      n.BackBufferColor = new Color(13, 22, 24);
      n.AmbientColor = Color.White;
    });
  }

  public override void ExitTree()
  {
    base.ExitTree();
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);
  }

  public override void Process(float delta)
  {
    base.Process(delta);
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }
}
