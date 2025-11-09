using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Slumber.Entities;

public class FallState : State
{

    Player player;

    public FallState (Player player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        
    }

    public override void Update(GameTime gameTime)
    {
        player.ApplyGravity();
        
        if (Core.Input.Keyboard.IsKeyDown(player.MoveLeftKey) || Core.Input.Keyboard.IsKeyDown(player.MoveRightKey))
        {
            RequestTransition("FallMoveState");
        }

        if (player.KinematicBase.Velocity.Y == 0)
        {
            RequestTransition("IdleState");
        }


        float accel = (MathF.Abs(0) > 0) ? player.PlayerInfo.Acceleration : player.PlayerInfo.Deceleration;
        player.KinematicBase.Velocity.X = player.MoveToward(player.KinematicBase.Velocity.X, 0, accel * Core.DeltaTime);
    }
    
    public override void OnExit()
    {
        
    }
}