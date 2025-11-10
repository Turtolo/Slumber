using ConstructEngine.Util;
using Microsoft.Xna.Framework;

namespace Slumber.Entities;

public class PlayerWallJumpState : State
{
    protected Player p;
    public PlayerWallJumpState(Player player) => p = player;

    public override void OnEnter()
    {
        p.PlayerInfo.canMove = false;

        if (p.PlayerInfo.dir == 1)
            p.KinematicBase.Velocity.X = -p.PlayerInfo.WallJumpHorizontalSpeed;
        else
            p.KinematicBase.Velocity.X = p.PlayerInfo.WallJumpHorizontalSpeed;

        p.KinematicBase.Velocity.Y = -p.PlayerInfo.WallJumpVerticalSpeed;

        Timer.Wait(0.12f, () => { p.PlayerInfo.canMove = true; });
    }

    public override void Update(GameTime gameTime)
    {
        if (p.KinematicBase.IsOnGround())
        {
            RequestTransition(nameof(PlayerIdleState));
        }
    }
}
