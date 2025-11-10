using Microsoft.Xna.Framework;

namespace Slumber.Entities;

public class PlayerFallState : State
{
    protected Player p;
    public PlayerFallState(Player player) => p = player;

    public override void OnEnter()
    {

    }

    public override void Update(GameTime gameTime)
    {
        p.AnimatedSprite.PlayAnimation(p._fallAnim, false);

        p.HandleHorizontalInput();

        if (p.KinematicBase.IsOnGround())
        {
            RequestTransition(nameof(PlayerIdleState));
        }

        if (p.KinematicBase.IsOnWall())
        {
            RequestTransition(nameof(PlayerWallSlideState));
        }
    }
}
