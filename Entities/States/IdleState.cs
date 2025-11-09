using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Slumber.Entities;

public class IdleState : State
{

    Player player;

    public IdleState (Player player)
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
            RequestTransition("RunState");
        }

        if ((Core.Input.Keyboard.WasKeyJustPressed(player.JumpKey) || player.PlayerInfo.bufferActivated) && player.KinematicBase.IsOnGround())
        {
            RequestTransition("JumpState");
        }

        float accel = (MathF.Abs(0) > 0) ? player.PlayerInfo.Acceleration : player.PlayerInfo.Deceleration;
        player.KinematicBase.Velocity.X = player.MoveToward(player.KinematicBase.Velocity.X, 0, accel * Core.DeltaTime);
    }
    
    public override void OnExit()
    {
        
    }
}