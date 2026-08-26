

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

public class Test : Scene
{
  public PropTest Props;

  public Player Player;

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

    new Player();

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
