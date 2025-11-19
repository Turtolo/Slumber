
namespace Slumber.Entities;

public class PlayerRunState : PlayerGroundedState
{
    public PlayerRunState(Player player) : base(player) { }

    public override void OnEnter()
    {
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        p.AnimatedSprite.PlayAnimation(p._runAnim, false);

        if (p.PlayerAxis == 0)
        {
            RequestTransition(nameof(PlayerIdleState));
            return;
        }

        if (Engine.Input.IsActionJustPressed("Attack"))
        {
            RequestTransition(nameof(PlayerAttackState));
            return;
        }
    }
}
