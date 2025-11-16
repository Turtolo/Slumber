using ConstructEngine;
using ConstructEngine.Util;
using Microsoft.Xna.Framework;

namespace Slumber.Entities
{
    public class PlayerJumpState : State
    {
        protected Player p;
        private bool jumpReleased;

        public PlayerJumpState(Player player) => p = player;

        public override void OnEnter()
        {
            p.PlayerInfo.bufferActivated = false;
            p.KinematicBase.Velocity.Y = p.PlayerInfo.JumpForce;
            jumpReleased = false;
        }

        public override void Update(GameTime gameTime)
        {
            p.AnimatedSprite.PlayAnimation(p._fallAnim, false);
            p.HandleHorizontalInput();

            if (!jumpReleased && Engine.Input.IsActionJustReleased("Jump") && p.KinematicBase.Velocity.Y < 0)
            {
                p.KinematicBase.Velocity.Y /= 4f;
                jumpReleased = true;
            }

            if ((Engine.Input.IsActionJustPressed("Jump") || p.PlayerInfo.bufferActivated) && !p.KinematicBase.IsOnGround())
            {
                p.PlayerInfo.bufferActivated = true;
                Timer.Wait(p.PlayerInfo.bufferTimer, () => p.PlayerInfo.bufferActivated = false);
            }

            if (p.KinematicBase.Velocity.Y > 0)
            {
                RequestTransition(nameof(PlayerFallState));
                return;
            }
        }
    }
}
