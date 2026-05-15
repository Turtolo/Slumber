public class PlayerInfo
{
  public PlayerInfo() { }
  public float MoveSpeed = 1000f;
  public float Acceleration = 3500f;
  public float Deceleration = 2500f;
  public float Gravity = 1300f;
  public float TerminalVelocity = 1200f;
  public float JumpForce = -350f;

  public float WallSlideGravity = 200f;
  public float WallJumpHorizontalSpeed = 200f;
  public float WallJumpVerticalSpeed = 300f;


  public bool justLeftLedge = false;
  public bool coyoteActivated = false;
  public float coyoteTimer = 0.2f;

  public bool jumpBuffered = false;
  public float jumpBufferTime = 0.2f;

  public bool attacking;

  public float attackBufferTime = 0.1f;
  public float attackBufferTimer = 0f;

  public int AttackCount = 0;
}