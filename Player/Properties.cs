namespace Slumber;

public record struct PlayerProperties 
{
  public float MoveSpeed { get; set; } = 130f;
  public float Acceleration { get; set; } = 3500f;
  public float Deceleration  { get; set; } = 2800f;

  public float BaseGravity { get; set; } = 950f;
  public float FallGravity { get; set; } = 1500f;

  public float CurrentTerminalVelocity { get; set; }
  public float InitialTerminalVelocity { get; set; } = 250f;
  public float SecondaryTerminalVelocity { get; set; } = 6400f;
  public bool ThresholdReached { get; set; }

  public float JumpForce { get; set; } = -360;

  public float WallSlideGravity { get; set; } = 20f;
  public float WallJumpHorizontalSpeed { get; set; } = 200f;
  public float WallJumpVerticalSpeed { get; set; } = 300f;

  public float DashVelocity { get; set; } = 300f;

  public TimeSpan CoyoteTime { get; set; } = TimeSpan.FromSeconds(0.6f);
  public TimeSpan JumpBufferTime { get; set; } = TimeSpan.FromSeconds(0.2f);
  public TimeSpan AttackBufferTime { get; set; } = TimeSpan.FromSeconds(0.08f);
  public TimeSpan DashDuration = TimeSpan.FromSeconds(0.2f);
  public TimeSpan DashCooldown = TimeSpan.FromSeconds(0.1f);

  public bool CanDash = true;

  public bool AllowControl { get; set; } = true;

  public Vector2 PlayerAxis { get; set; }

  public bool CanTakeDamage { get; set; } = true;

  public bool JumpReleased { get; set; } = false;
  public bool WallSlideTriggered = false;

  public bool JumpBuffered { get; set; } = false;
  public bool CanCoyoteJump { get; set; } = false;
  public bool WasOnFloor { get; set; } = false;

  public int AttackCounter { get; set; }
  public bool AttackBuffer { get; set; }
  public bool IsAttacking { get; set; } = false;

  public bool IsDashing { get; set; } = false;

  public float PreviousY { get; set; }

  public PlayerProperties() {}

}
