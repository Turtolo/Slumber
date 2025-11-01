

using Microsoft.Xna.Framework;

public class PlayerInfo
{
    public PlayerInfo() { }

    public bool attacking;
    
    public float Gravity = 1300f;
    public float JumpForce = -350f;
    public float MoveSpeed = 100f;


    public float bufferTimer = 0.2f;
    public float WallJumpHorizontalSpeed = 200;
    public float WallJumpVerticalSpeed = 300;
    
    public bool bufferActivated;
    public bool dir = true;
    public int AttackCount = 0;
    public int textureOffset;
    public bool _upwardAir;
    public bool canMove = true;
    public bool wallSlide;
    public bool falling;
}