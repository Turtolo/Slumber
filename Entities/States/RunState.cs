using System;
using ConstructEngine;
using Microsoft.Xna.Framework;
using Slumber.Entities;

public class RunState : State
{

    Player player;

    public RunState (Player player)
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

        if (Core.Input.Keyboard.IsKeyDown(player.MoveLeftKey) || Core.Input.Keyboard.IsKeyDown(player.MoveRightKey)) { }

        else
        {
            RequestTransition("IdleState");
        }

        if ((Core.Input.Keyboard.WasKeyJustPressed(player.JumpKey) || player.PlayerInfo.bufferActivated) && player.KinematicBase.IsOnGround())
        {
            RequestTransition("JumpState");
        }

        if (player.PlayerInfo.VariableJump && Core.Input.Keyboard.WasKeyJustReleased(player.JumpKey) && player.KinematicBase.Velocity.Y < 0)
        {
            RequestTransition("HalfJumpState");
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