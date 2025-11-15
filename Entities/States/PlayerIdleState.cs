using ConstructEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Slumber.Entities;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void OnEnter()
    {
    }

    public override void Update(GameTime gameTime)
    {
        p.AnimatedSprite.PlayAnimation(p._idleAnim, false);
        
        base.Update(gameTime);

        if (Engine.Input.IsActionPressed("MoveLeft") || Engine.Input.IsActionPressed("MoveRight"))
        {
            RequestTransition(nameof(PlayerRunState));
            return;
        }

        if (Engine.Input.IsActionJustPressed("Attack"))
        {
            RequestTransition(nameof(PlayerAttackState));
            return;
        }
    }
}
