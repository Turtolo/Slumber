using RenderingLibrary;

namespace Slumber.Entities;

public class Player : KinematicBody2D
{
    private TextureAtlas _atlas;

    public Animation _runAnim;
    public Animation _idleAnim;
    public Animation _fallAnim;
    public Animation _attackAnim1;
    public Animation _attackAnim2;

    public int AttackColliderOffset;

    public PlayerInfo PlayerInfo = new();

    Area2D TakeDamageArea;

    private PlayerUI Screen;
    private Pausemenu pauseMenu;
    public AnimatedSprite AnimatedSprite;
    private StateController _stateController;

    public int PlayerAxis;

    public Player() {}

    public override void Load()
    {
        
        Screen = new PlayerUI();
        pauseMenu = new Pausemenu();

        _atlas = TextureAtlas.FromFile(Engine.Content, "Assets/Atlas/Player/player-atlas.xml", "Assets/Animations/Player/PlayerModel3Atlas");

        _runAnim = _atlas.GetAnimation("run-animation");
        _idleAnim = _atlas.GetAnimation("idle-animation");
        _fallAnim = _atlas.GetAnimation("fall-animation");
        _attackAnim1 = _atlas.GetAnimation("attack-animation-1");
        _attackAnim2 = _atlas.GetAnimation("attack-animation-2");

        AnimatedSprite = _atlas.CreateAnimatedSprite("idle-animation");
        AnimatedSprite.LayerDepth = 0.5f;


        Shape = new RectangleShape2D(400, 150, 10, 25);

        TakeDamageArea = new(Shape);

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
        

        _stateController = new StateController(idle,
        [
            grounded, idle, run, attack, jump, fall, wallSlide, wallJump
        ]);
    }

    public override void Update(GameTime gameTime)
    {   
        UpdateKinematicBody();

        PlayerAxis = Engine.Input.GetAxis("MoveLeft", "MoveRight");

        ApplyGravity();

        _stateController.Update(gameTime);

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
