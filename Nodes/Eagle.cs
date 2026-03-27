namespace Slumber;

public class Eagle : Node2D
{
    public AnimatedSprite2D Sprite;

    public override void OnEnter()
    {
        base.OnEnter();

        var animations = AsepriteLoader.LoadAnimations(
            Engine.Resource.Load<MTexture>("Graphics/Atlas/EagleAtlas"),
            PathHelper.Combine("Raw/Raw/Eagle.json")
        );

        Sprite = Engine.Tree.Create<AnimatedSprite2D>().SetProperties(n =>
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


}