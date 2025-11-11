using Microsoft.Xna.Framework;

namespace Slumber.Entities;

public class PlayerAttackState : PlayerGroundedState
{
    public PlayerAttackState(Player player) : base(player) { }

    public override void OnEnter()
    {
        p.PlayerInfo.AttackCount++;
        p.PlayerInfo.attacking = true;
        p.DamageArea.Enabled = true;

        p.AnimatedSprite.PlayAnimation(p._attackAnim1, false);

        //if (p.PlayerInfo.AttackCount == 1)
            //p.AnimatedSprite.PlayAnimation(p._attackAnim1, false);
        //else
            //p.AnimatedSprite.PlayAnimation(p._attackAnim2, false);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (p.AnimatedSprite.finished)
        {
            p.PlayerInfo.attacking = false;
            p.DamageArea.Enabled = false;
            RequestTransition(nameof(PlayerIdleState));
        }
    }

    public override void OnExit()
    {
        p.PlayerInfo.attacking = false;
        p.DamageArea.Enabled = false;
    }
}
