

using System.IO;
using System.Linq;
using DotTiled.Serialization;
using MonoTile;

namespace Slumber;

public class Test : Scene
{
  RectangleShape2D Shape1;
  RectangleShape2D Shape2;
  RectangleShape2D Shape3;
  RectangleShape2D Shape4;
  RectangleShape2D Shape5;
  RectangleShape2D Shape6;

  RayCastShape2D LeftRay;
  RayCastShape2D RightRay;

  StaticBody2D S1;
  Raycast2D LR;

  public override void EnterTree()
  {
    base.EnterTree();

    Shape1 = new RectangleShape2D(new Extent(100, 10));

    S1 = new StaticBody2D().Set(n =>
    {
      n.AddChild(new CollisionShape2D().Set(n => n.Shape = Shape1));
      n.Position = new Vector2(-50, 0);
    });

    LeftRay = new RayCastShape2D(new Vector2(-10, 10));

    LR = new Raycast2D().Set(n =>
    {
      n.Shape = LeftRay;
      n.Position = new Vector2(-10, -5);
    });
  }

  public override void ExitTree()
  {
    base.ExitTree();
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);

    //Console.WriteLine((LeftRay.CheckIntersections(Shape1, new Vector2(-10, -5), new Vector2(-50, 0), out _, out _)));
    
    if (LR.IsColliding(out _, out _))
      Console.WriteLine("True");
    else
      Console.WriteLine("False");
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
