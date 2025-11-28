using System.Collections.Generic;
using RenderingLibrary;

namespace Slumber.Entities;

public class Player : KinematicBody2D
{
    public Animation _runAnim;
    public Animation _idleAnim;
    public Animation _fallAnim;
    public Animation _attackAnim1;
    public Animation _attackAnim2;

    public int AttackColliderOffset;

    public PlayerInfo PlayerInfo = new();

    Area2D TakeDamageArea;

    public bool Dead;

    private PlayerUI Screen;
    private Pausemenu pauseMenu;
    public AnimatedSprite AnimatedSprite;
    public StateController StateController;

    public int PlayerAxis;

    public Player(NodeConfig config) : base(config) {}

    public override void Load()
    {
        Screen = new PlayerUI();
        pauseMenu = new Pausemenu();

        MTexture PlayerTexture = new("Assets/Animations/Player/PlayerModel3Atlas");

        List<MTexture> IdleAnimSheet = SpriteSlicer.Slice(PlayerTexture, 128, 128, 0, 5, 0, 0);
        List<MTexture> RunAnimSheet = SpriteSlicer.Slice(PlayerTexture, 128, 128, 0, 5, 1, 1);
        List<MTexture> FallAnimSheet = SpriteSlicer.Slice(PlayerTexture, 128, 128, 0, 4, 2, 2);
        List<MTexture> AttackAnimSheet = SpriteSlicer.Slice(PlayerTexture, 128, 128, 0, 10, 3, 3);



        _runAnim = new(RunAnimSheet, TimeSpan.FromMilliseconds(100));
        _idleAnim = new(IdleAnimSheet, TimeSpan.FromMilliseconds(100));
        _fallAnim = new(FallAnimSheet, TimeSpan.FromMilliseconds(100));
        _attackAnim1 = new(AttackAnimSheet, TimeSpan.FromMilliseconds(100));
        _attackAnim2 = new(AttackAnimSheet, TimeSpan.FromMilliseconds(100));
        
        AnimatedSprite = new(_idleAnim);
        AnimatedSprite.LayerDepth = 0.5f;

        

        Shape.Width = 10;
        Shape.Height = 25;

        TakeDamageArea = new(new NodeConfig{Shape = Shape, Parent = this, Name = "PlayerTakeDamageArea"});

        CircleShape2D attackCircle = new(0, 0, 30);
        
        var grounded = new PlayerGroundedState(this);
        var idle = new PlayerIdleState(this);
        var run = new PlayerRunState(this);
        var attack = new PlayerAttackState(this);
        var jump = new PlayerJumpState(this);
        var fall = new PlayerFallState(this);
        var wallSlide = new PlayerWallSlideState(this);
        var wallJump = new PlayerWallJumpState(this);

        idle.SetParent(grounded);
        run.SetParent(grounded);
        attack.SetParent(grounded);
        

        StateController = new StateController(idle,
        [
            grounded, idle, run, attack, jump, fall, wallSlide, wallJump
        ]);
    }

    public override void Update(GameTime gameTime)
    {   
        UpdateKinematicBody();

        PlayerAxis = Engine.Input.GetAxis("MoveLeft", "MoveRight");

        ApplyGravity();

        StateController.Update(gameTime);

        SaveManager.PlayerData.CurrentPosition = Location.ToVector2();

        FlipSprite();

        AnimatedSprite.Position = new Vector2(Location.X - 64 + PlayerInfo.textureOffset, Location.Y - 55);
        AnimatedSprite.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {   
        Engine.DrawManager.Draw(AnimatedSprite);
    }

    public void ApplyGravity()
    {
        if (!IsOnGround())
        {
            Velocity.Y = MathF.Min(
                Velocity.Y + PlayerInfo.Gravity * Engine.DeltaTime,
                PlayerInfo.TerminalVelocity
            );
        }
        else if (Velocity.Y > 0)
        {
            Velocity.Y = 0;
        }
    }

    public void HandleHorizontalInput()
    {
        float targetSpeed = 0f;

        if (Engine.Input.IsActionPressed("MoveLeft") && !Engine.Input.IsActionPressed("MoveRight"))
        {
            targetSpeed = -PlayerInfo.MoveSpeed;
            PlayerInfo.dir = -1;
        }
        else if (Engine.Input.IsActionPressed("MoveRight") && !Engine.Input.IsActionPressed("MoveLeft"))
        {
            targetSpeed = PlayerInfo.MoveSpeed;
            PlayerInfo.dir = 1;
        }

        float accel = (MathF.Abs(targetSpeed) > 0) ? PlayerInfo.Acceleration : PlayerInfo.Deceleration;
        Velocity.X = MoveToward(Velocity.X, targetSpeed, accel * Engine.DeltaTime);
    }

    private float MoveToward(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta) return target;
        return current + MathF.Sign(target - current) * maxDelta;
    }

    public void FlipSprite()
    {
        if (PlayerInfo.dir == 1)
        {
            AttackColliderOffset = 15;
            PlayerInfo.textureOffset = 0;
            AnimatedSprite.Effects = SpriteEffects.None;
        }
        else
        {
            AttackColliderOffset = -38;
            PlayerInfo.textureOffset = 8;
            AnimatedSprite.Effects = SpriteEffects.FlipHorizontally;
        }
    }
}
