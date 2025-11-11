using ConstructEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

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

        if (!Core.Input.IsActionPressed("MoveLeft") && !Core.Input.IsActionPressed("MoveRight"))
        {
            RequestTransition(nameof(PlayerIdleState));
            return;
        }

        if (Core.Input.IsActionJustPressed("Attack"))
        {
            RequestTransition(nameof(PlayerAttackState));
            return;
        }
    }
}
