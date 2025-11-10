using Microsoft.Xna.Framework;

namespace Slumber.Entities;

public class PlayerJumpState : State
{
    protected Player p;
    public PlayerJumpState(Player player) => p = player;

    public override void OnEnter()
    {
        p.KinematicBase.Velocity.Y = p.PlayerInfo.JumpForce;
    }

    public override void Update(GameTime gameTime)
    {
        p.AnimatedSprite.PlayAnimation(p._fallAnim, false);
        
        p.HandleHorizontalInput();

        if (p.KinematicBase.Velocity.Y > 0)
        {
            RequestTransition(nameof(PlayerFallState));
            return;
        }
    }
}
