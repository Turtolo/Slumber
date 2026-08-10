using System.Linq;

namespace Slumber;

public partial class Player : KinematicBody2D
{
  public void HandleDash()
  {
    if (!Properties.CanDash)
      return;

    if (Core.Input.IsActionJustPressed("Dash"))
      Dash();
  }

  public void Dash()
  {
    Properties.IsDashing = true;

    Velocity.X = Properties.DashVelocity * Properties.PlayerDirection;
    Velocity.Y = 0;

    Await.Span(Properties.DashDuration, () =>
    {
      Properties.IsDashing = false;
      Properties.CanDash = false;
      Await.Span(Properties.DashCooldown, () => Properties.CanDash = true);
    });
  }

  public void HandleMovementInput()
  {
    if (!Properties.AllowControl || Properties.IsDashing)
      return;

    float targetSpeed = Properties.MoveSpeed * Properties.PlayerAxis.X;

    if (targetSpeed != 0)
      Velocity.X = MoveToward(Velocity.X, targetSpeed, Properties.Acceleration);
  }


  public void FlipSprite()
  {
    if (Properties.PlayerDirection > 0)
    {
      Sprite.SpriteEffects = SpriteEffects.None;
      AttackArea.Position = new Vector2(40, 5);
    }
    else if (Properties.PlayerDirection < 0)
    {
      Sprite.SpriteEffects = SpriteEffects.FlipHorizontally;
      AttackArea.Position = new Vector2(-30, 5);
    }
  }

  public void HandleDeceleration(float delta)
  {
    if (!Properties.AllowControl || Properties.IsDashing)
      return;

    Velocity.X = Properties.PlayerAxis.X == 0 ? MoveToward(Velocity.X, 0, Properties.Deceleration * delta) : Velocity.X;
  }

  public float MoveToward(float current, float target, float maxDelta)
  {
    if (MathF.Abs(target - current) <= maxDelta)
      return target;

    return current + MathF.Sign(target - current) * maxDelta;
  }

  #region Jumping and Gravity

  public void ApplyGravity(float delta)
  {
    if (Properties.PreviousY > 0 && IsOnFloor)
      Land();

    Properties.PreviousY = Velocity.Y;

    if (Properties.IsDashing)
      return;

    if (!IsOnFloor)
    {
      float activeGravity = Velocity.Y < 0 ? Properties.BaseGravity : Properties.FallGravity;

      float alpha = Properties.CurrentTerminalVelocity > Properties.InitialTerminalVelocity ? 0.3f : 0.02f;

      if (GroundCheck.IsColliding())
        Properties.CurrentTerminalVelocity = Properties.InitialTerminalVelocity;
      else if (Properties.CurrentTerminalVelocity != Properties.SecondaryTerminalVelocity)
        Properties.CurrentTerminalVelocity = MathHelper.Lerp(Properties.CurrentTerminalVelocity, Properties.SecondaryTerminalVelocity, 0.1f);
      
      if (Velocity.Y >= 700f)
        Properties.ThresholdReached = true;

      Velocity.Y = MathF.Min(
        Velocity.Y + activeGravity * delta,
        Properties.CurrentTerminalVelocity
      );
    }
    else if (Velocity.Y > 0)
    {
      Velocity.Y = 0;
    }
  }

  public void Land()
  {
  }

  public void HandleCoyoteTime()
  {
    if (Properties.WasOnFloor && !IsOnFloor && Velocity.Y >= 0f)
    {
      Properties.CanCoyoteJump = true;
      Await.Span(Properties.CoyoteTime, () => Properties.CanCoyoteJump = false);
    }

    if (IsOnFloor)
      Properties.CanCoyoteJump = false;

    Properties.WasOnFloor = IsOnFloor;
  }

  #endregion

  #region Wall Interaction

  public bool CanWall()
  {
    return Properties.PlayerAxis.X != 0 && IsOnWall && Velocity.Y > 0;
  }

  public void HandleWallSlide()
  {

    if (!Properties.WallSlideTriggered)
      return;

    if (!IsOnWall || IsOnFloor)
      Properties.WallSlideTriggered = false;

    Velocity.Y = MathF.Min(
      Velocity.Y + Properties.WallSlideGravity,
      Properties.WallSlideGravity
    );

    if (Core.Input.IsActionJustPressed("Jump"))
      WallJump();
  }

  public void WallJump()
  {
    Properties.AllowControl = false;
    Await.Span(TimeSpan.FromSeconds(0.06f), () => Properties.AllowControl = true);

    if (Properties.PlayerDirection == 1)
      Velocity.X = -Properties.WallJumpHorizontalSpeed;
    else if (Properties.PlayerDirection == -1)
      Velocity.X = Properties.WallJumpHorizontalSpeed;

    Velocity.Y = -Properties.WallJumpVerticalSpeed;
  }

  #endregion

  #region Attack

  public void Kill()
  {
    Core.Token.Anchor.ReloadCurrentAnchor();
  }

  public void HandleDamage()
  {
    if (Properties.Health <= 0)
    {
      Kill();
    }

    if (!Properties.CanTakeDamage)
      return;

    var areas = TakeDamageArea.AreasEntered();

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

  public void TakeDamage(int damage, int dir)
  {
    Sprite.Shader.Parameters["enabled"].SetValue(1);
    Properties.CanTakeDamage = false;

    Properties.Health -= damage;

    HealthIcons.LastOrDefault().Frame = 1;
    HealthIcons.RemoveAt(HealthIcons.Count - 1);

    Properties.AllowControl = false;

    Velocity = new Vector2(300 * dir, -300);

    Core.Time.TimeScale = 0f;

    Await.Span(TimeSpan.FromSeconds(0.2), () =>
    {
      Core.Time.TimeScale = 1f;
      Properties.AllowControl = true;
      Sprite.Shader.Parameters["enabled"].SetValue(0);
      Properties.CanTakeDamage = true;
    }, true);
  }

  public void HandleAttack()
  {
    if (Core.Input.IsActionJustPressed("Attack"))
    {
      if (!Properties.IsAttacking)
      {
        Attack();
      }
      else
      {
        BufferAttack();
      }
    }

    if (Properties.IsAttacking && Sprite.IsFinished)
    {
      AttackArea.Get<CollisionShape2D>().Disabled = true;
      Properties.IsAttacking = false;

      if (Properties.AttackBuffer)
      {
        Properties.AttackBuffer = false;
        Attack();
      }
    }
  }

  public void Attack()
  {
    AttackArea.Get<CollisionShape2D>().Disabled = false;
    Properties.AttackCounter++;
    Properties.IsAttacking = true;
  }

  public void BufferAttack()
  {
    if (Properties.AttackBuffer)
      return;

    Properties.AttackBuffer = true;

    Await.Span(Properties.AttackBufferTime, () =>
    {
      Properties.AttackBuffer = false;
    });
  }

  #endregion
}
