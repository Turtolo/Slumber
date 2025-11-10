using ConstructEngine;
using Microsoft.Xna.Framework;

namespace Slumber.Entities;

public class PlayerGroundedState : State
{
    protected Player p;
    public PlayerGroundedState(Player player) => p = player;

    public override void Update(GameTime gameTime)
    {
        p.HandleHorizontalInput();

        if (!p.KinematicBase.IsOnGround())
        {
            RequestTransition(nameof(PlayerFallState));
            return;
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(p.JumpKey))
        {
            RequestTransition(nameof(PlayerJumpState));
            return;
        }

        base.Update(gameTime);
    }
}
