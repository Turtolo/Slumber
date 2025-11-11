using ConstructEngine;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;

namespace Slumber.Entities;

public class PlayerWallSlideState : State
{
    protected Player p;
    public PlayerWallSlideState(Player player) => p = player;

    public override void OnEnter()
    {
        p.PlayerInfo.wallSlide = true;
        p.KinematicBase.Velocity.Y = 0;
    }

    public override void Update(GameTime gameTime)
    {
        p.KinematicBase.Velocity.Y = 0;
        
        if (!p.KinematicBase.IsOnWall() || p.KinematicBase.IsOnGround())
        {
            RequestTransition(nameof(PlayerFallState));
            return;
        }

        if (Core.Input.IsActionJustPressed("Jump"))
        {
            RequestTransition(nameof(PlayerWallJumpState));
            return;
        }

    }

    public override void OnExit()
    {
        p.PlayerInfo.wallSlide = false;
    }
}
