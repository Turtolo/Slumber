

using System.Collections.Generic;
using System.IO;
using System.Linq;
using DotTiled.Serialization;
using Gum.Forms.Controls;
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

public class Test : Scene
{
  public PropTest Props;

  public AnimatedSprite2D Transition;

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
      n.Color = Color.Red;
      n.Scale = new Vector2(100, 100);
    });

    var transAn = AsepriteLoader.LoadAnimations(
        new TextureRegion(Core.Resource.Load<Texture2D>("Graphics/Transition"), new Rectangle(0, 0, 28, 1)),
        PathTools.Combine("Raw/Raw/Transition.json") 
    );

    Transition = Core.Token.Create<AnimatedSprite2D>().Set(n =>
    {
      n.Atlas = transAn;
      //n.Rounded = true;
      n.Seperated = true;
      n.Position = new Vector2(320, 180);
      n.Scale = new Vector2(640, 360);
      n.Depth = 20;
    });

    Transition.Detach();
    
    var b = new Keyboard();
    b.AddToRoot();

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

    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.I))
      Transition.PlayAnimation("In");
    if (Core.Input.Keyboard.WasKeyJustPressed(Keys.O))
      Transition.PlayAnimation("Out");
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }
}
