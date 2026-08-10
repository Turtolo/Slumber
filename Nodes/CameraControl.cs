using System.Collections.Generic;
using System.Linq;

namespace Slumber;

public class CameraControl : Node2D
{
  public List<Area2D> LimitAreas { get; set; } = new();

  public override void _Process(float delta)
  {
    base._Process(delta);

    for (int i = 0; i < LimitAreas.Count; i++)
    {
      Area2D area = LimitAreas[i];

      if (area.GetAnyBody() is Player)
      {
        var shape = area.CollisionShapes.FirstOrDefault();

        Core.Token.Get<PixelCamera>()?.Limit = shape.Shape.ToRectangle(area.Transform.Global.Position.ToPoint());
      }
    }
  }
}

