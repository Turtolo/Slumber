using System.Linq;
using Microsoft.VisualBasic;

namespace Slumber;

public class Eagle : Node2D
{
    public Sprite2D Sprite;

    public Node2D TargetNode;

    public int[,] CSV;

    public Path2D Path;

    public override void OnEnter()
    {
        base.OnEnter();

        Path = Engine.Tree.Create<Path2D>().SetProperties(n =>
        {
            n.Target = this;
        });

        var animations = AsepriteLoader.LoadAnimations(
            Engine.Resource.Load<MTexture>("Graphics/Atlas/EagleAtlas"),
            PathHelper.Combine("Raw/Raw/Eagle.json")
        );

        Sprite = Engine.Tree.Create<Sprite2D>().SetProperties(n =>
        {
            n.SetParent(this);
            n.Texture = Engine.Pixel;
        });
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);

        //Sprite.PlayAnimation("Flying");

        if (Engine.Input.Keyboard.WasKeyJustPressed(Keys.F))
            UpdatePath();
    }

    public override void ProcessUpdate(float delta)
    {
        base.ProcessUpdate(delta);
    }

    public override void SubmitCall()
    {
        base.SubmitCall();
    }

    public override void OnExit()
    {
        base.OnExit();
    }


    public void UpdatePath()
    {
        if (CSV == null || TargetNode == null)
            return;

        var tarCords = TargetNode.GlobalPosition.ToPoint();
        var thisCords = GlobalPosition.ToPoint();

        var startInPlaneCords = new Point(thisCords.X / 16, thisCords.Y / 16);
        var goalInPlaneCords = new Point(tarCords.X / 16, tarCords.Y / 16).FindNearestSafeTile(CSV);

        var path = AStar.GetPath(CSV, startInPlaneCords, goalInPlaneCords).ToWorldCords(16).ToVec2().ToArray();
                
        Path.SetPath(path);

    }

}