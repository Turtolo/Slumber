using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Slumber.Entities;
using Timer = ConstructEngine.Util.Timer;

public class TakeDamageState : State
{

    Player player;

    public TakeDamageState (Player player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.KinematicBase.Velocity.X = 0;
        player.KinematicBase.Velocity.Y = 0;

        player.KinematicBase.Velocity.X = 250 * player.PlayerInfo.dir;
        player.KinematicBase.Velocity.Y = -250;

        Timer.Wait(0.025f, () => RequestTransition("IdleState"));

    }

    public override void Update(GameTime gameTime)
    {
        
    }
    
    public override void OnExit()
    {
        
    }
}