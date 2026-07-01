using System.Linq;

namespace Slumber;

public static class PlayerFunctions
{
    private static Player p => Core.Token.Get<Player>();

    public static void HandleDash()
    {
        if (!p._canDash)
            return;

        if (Core.Input.IsActionJustPressed("Dash"))
            Dash();
    }

    public static void Dash()
    {
        p._isDashing = true;

        p.Velocity.X = p.DashVelocity * p.PlayerDirection;
        p.Velocity.Y = 0;

        Await.Span(p.DashDuration, () =>
        {
            p._isDashing = false;
            p._canDash = false;
            Await.Span(p.DashCooldown, () => p._canDash = true);
        });
    }

    public static void HandleMovementInput()
    {
        if (!p.AllowControl || p._isDashing)
            return;

        float targetSpeed = p.MoveSpeed * p.PlayerAxis.X;

        if (targetSpeed != 0)
            p.Velocity.X = MoveToward(p.Velocity.X, targetSpeed, p.Acceleration);
    }

    public static void HandleDeceleration(float delta)
    {
        if (!p.AllowControl || p._isDashing)
            return;

        p.Velocity.X = p.PlayerAxis.X == 0 ? MoveToward(p.Velocity.X, 0, p.Deceleration * delta) : p.Velocity.X;
    }

    public static float MoveToward(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta)
            return target;

        return current + MathF.Sign(target - current) * maxDelta;
    }

    #region Jumping and Gravity

    public static void ApplyGravity(float delta)
    {
        if (p._prevY > 0 && p.IsOnFloor)
            Land();

        p._prevY = p.Velocity.Y;

        if (p._isDashing)
            return;
        
        if (!p.IsOnFloor)
        {
            float activeGravity = p.Velocity.Y < 0 ? p.BaseGravity : p.FallGravity;
            
            if (p.GroundCheck.IsColliding())
                p.CurrentTerminalVelocity = p.InitialTerminalVelocity;
            else if (p.CurrentTerminalVelocity != p.SecondaryTerminalVelocity)
                p.CurrentTerminalVelocity = MathHelper.Lerp(p.CurrentTerminalVelocity, p.SecondaryTerminalVelocity, 0.1f);

            p.Velocity.Y = MathF.Min(
                p.Velocity.Y + activeGravity * delta,
                p.CurrentTerminalVelocity
            );
        }
        else if (p.Velocity.Y > 0)
        {
            p.Velocity.Y = 0;
        }
    }

    public static void Land()
    {
    }

    public static void HandleCoyoteTime()
    {
        if (p.wasOnFloor && !p.IsOnFloor && p.Velocity.Y >= 0f)
        {
            p.canCoyoteJump = true;
            Await.Span(p.CoyoteTime, () => p.canCoyoteJump = false);
        }

        if (p.IsOnFloor)
            p.canCoyoteJump = false;

        p.wasOnFloor = p.IsOnFloor;
    }

    #endregion

    #region Wall Interaction

    public static void HandleWallSlide()
    {
        if (p.PlayerAxis.X != 0 && p.IsOnWall && p.Velocity.Y > 0)
            p.wallSlideTriggered = true;

        if (!p.wallSlideTriggered)
            return;

        if (!p.IsOnWall || p.IsOnFloor)
            p.wallSlideTriggered = false;

        p.Velocity.Y = MathF.Min(
            p.Velocity.Y + p.WallSlideGravity,
            p.WallSlideGravity
        );

        if (Core.Input.IsActionJustPressed("Jump"))
            WallJump();
    }

    public static void WallJump()
    {
        p.AllowControl = false;
        Await.Span(TimeSpan.FromSeconds(0.06f), () => p.AllowControl = true);

        if (p.PlayerDirection == 1)
            p.Velocity.X = -p.WallJumpHorizontalSpeed;
        else if (p.PlayerDirection == -1)
            p.Velocity.X = p.WallJumpHorizontalSpeed;

        p.Velocity.Y = -p.WallJumpVerticalSpeed;
    }

    #endregion

    #region Attack

    public static void HandleDamage()
    {
        if (p.Health <= 0)
        {
            Core.Tree.ReloadCurrentScene();
        }

        if (!p.CanTakeDamage)
            return;

        var areas = p.TakeDamageArea.AreasEntered();

        if (areas.Any())
        {
            foreach (var area in areas)
            {
                if (area.Name == "LeftArea")
                {
                    TakeDamage(1, -1);
                    return;
                }

                if (area.Name == "RightArea")
                {
                    TakeDamage(1, 1);
                    return;
                }
            }
        }
    }

    public static void TakeDamage(int damage, int dir)
    {
        p.Sprite.Shader.Parameters["enabled"].SetValue(1);
        p.CanTakeDamage = false;

        p.Health -= damage;

        p.healthIcons.LastOrDefault().Frame = 1;
        p.healthIcons.RemoveAt(p.healthIcons.Count - 1);

        p.AllowControl = false;

        p.Velocity = new Vector2(300 * dir, -300);

        Core.Time.TimeScale = 0f;

        Await.Span(TimeSpan.FromSeconds(0.2), () =>
        {
            Core.Time.TimeScale = 1f;
            p.AllowControl = true;
            p.Sprite.Shader.Parameters["enabled"].SetValue(0);
            p.CanTakeDamage = true;
        }, true);
    }

    public static void HandleAttack()
    {
        if (Core.Input.IsActionJustPressed("Attack"))
        {
            if (!p.isAttacking)
            {
                Attack();
            }
            else
            {
                BufferAttack();
            }
        }

        if (p.isAttacking && p.Sprite.IsFinished)
        {
            p.AttackArea.Get<CollisionShape2D>().Disabled = true;
            p.isAttacking = false;

            if (p.attackBuffer)
            {
                p.attackBuffer = false;
                Attack();
            }
        }
    }

    public static void Attack()
    {
        p.AttackArea.Get<CollisionShape2D>().Disabled = false;
        p.attackCounter++;
        p.isAttacking = true;
    }

    public static void BufferAttack()
    {
        if (p.attackBuffer)
            return;

        p.attackBuffer = true;

        Await.Span(p.AttackBufferTime, () =>
        {
            p.attackBuffer = false;
        });
    }

    #endregion
}
