using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Slumber.Entities;

public class HalfJumpState : State
{

    Player player;

    public HalfJumpState (Player player)
    {
        this.player = player;
    }

    public override void OnEnter()
    {
        player.KinematicBase.Velocity.Y *= 0.5f;
    }

    public override void Update(GameTime gameTime)
    {
        player.ApplyGravity();

        if (player.KinematicBase.Velocity.Y > 0)
        {
            if (Core.Input.Keyboard.IsKeyDown(player.MoveLeftKey) || Core.Input.Keyboard.IsKeyDown(player.MoveRightKey))
            {
                RequestTransition("FallMoveState");
            }
            else
            {
                RequestTransition("FallState");
            }
        }

        HandleHorizontalInput();

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