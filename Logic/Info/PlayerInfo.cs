

using Microsoft.Xna.Framework;

public class PlayerInfo
{
    public PlayerInfo() { }

    public bool attacking;

    public float attackBufferTime = 0.1f;
    public float attackBufferTimer = 0f;
    public float MoveSpeed = 100f;
    public float Acceleration = 3500f;
    public float Deceleration = 2500f;
    public float Gravity = 1300f;
    public float TerminalVelocity = 1200f;
    public float JumpForce = -350f;
    public bool VariableJump = true;
    public bool bufferActivated = false;
    public float bufferTimer = 0.2f;

    public float WallJumpHorizontalSpeed = 200;
    public float WallJumpVerticalSpeed = 300;
    public int dir = 1;
    public int AttackCount = 0;
    public int textureOffset;
    public bool canMove = true;
    public bool wallSlide;
}