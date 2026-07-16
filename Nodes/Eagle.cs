using System.Linq;
using Microsoft.VisualBasic;

namespace Slumber;

public class Eagle : Node2D
{
  public AnimatedSprite2D Sprite;

  public Node2D TargetNode;

  public int[,] CSV;

  public Path2D Path;

  private float pathTimer = 0f;
  private float pathInterval = 0.5f;

  public override void EnterTree()
  {
    base.EnterTree();

    Path = Core.Token.Create<Path2D>().Set(n =>
    {
      n.Target = this;
    });

    var animations = AsepriteLoader.LoadAnimations(
        Core.Resource.Load<MTexture>("Graphics/Atlas/EagleAtlas"),
        PathTools.Combine("Raw/Raw/Eagle.json")
    );

    Sprite = Core.Token.Create<AnimatedSprite2D>().Set(n =>
    {
      n.SetParent(this);
      n.Atlas = animations;
      n.IsLooping = true;
    });
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);

    Sprite.PlayAnimation("Flying");


    if (TargetNode == null)
      return;

    pathTimer -= delta;

    if (pathTimer <= 0f)
    {
      UpdatePath();
      pathTimer = pathInterval;
    }
  }

  public override void Process(float delta)
  {
    base.Process(delta);
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }

  public override void ExitTree()
  {
    base.ExitTree();
  }


  public void UpdatePath()
  {
    if (CSV == null || TargetNode == null)
      return;

    var tarCords = TargetNode.Transform.Global.Position.ToPoint();
    var thisCords = Transform.Global.Position.ToPoint();

    var startInPlaneCords = new Point(thisCords.X / 16, thisCords.Y / 16);
    var goalInPlaneCords = new Point(tarCords.X / 16, tarCords.Y / 16).FindNearestSafeTile(CSV);

    var path = AStar.GetPath(CSV, startInPlaneCords, goalInPlaneCords).ToWorldCords(16, 16).ToVec2().ToArray();

    Path.SetPath(path);

  }

}
