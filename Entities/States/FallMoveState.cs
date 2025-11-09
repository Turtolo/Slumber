using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Slumber.Entities;

public class FallMoveState : State
{

    Player player;

    public FallMoveState (Player player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
    
    }

    public override void Update(GameTime gameTime)
    {
        player.ApplyGravity();

        HandleHorizontalInput();
        
        if (Core.Input.Keyboard.IsKeyDown(player.MoveLeftKey) || Core.Input.Keyboard.IsKeyDown(player.MoveRightKey))
        {
            
        }

        else
        {
            RequestTransition("FallState");
        }

        if (player.KinematicBase.Velocity.Y == 0)
        {
            RequestTransition("IdleState");
        }

    }
    
    private void HandleHorizontalInput()
    {
        float targetSpeed = 0f;

        if (Core.Input.Keyboard.IsKeyDown(player.MoveLeftKey) && !Core.Input.Keyboard.IsKeyDown(player.MoveRightKey))
        {
            targetSpeed = -player.PlayerInfo.MoveSpeed;
            player.PlayerInfo.dir = -1;
        }
        else if (Core.Input.Keyboard.IsKeyDown(player.MoveRightKey) && !Core.Input.Keyboard.IsKeyDown(player.MoveLeftKey))
        {
            targetSpeed = player.PlayerInfo.MoveSpeed;
            player.PlayerInfo.dir = 1;
        }

        float accel = (MathF.Abs(targetSpeed) > 0) ? player.PlayerInfo.Acceleration : player.PlayerInfo.Deceleration;
        player.KinematicBase.Velocity.X = player.MoveToward(player.KinematicBase.Velocity.X, targetSpeed, accel * Core.DeltaTime);
    }
    
    public override void OnExit()
    {
        
    }
}