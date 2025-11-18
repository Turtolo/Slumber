namespace Slumber.Entities;

public class Player : KinematicEntity, IKinematicEntity
{
    private TextureAtlas _atlas;

    public Animation _runAnim;
    public Animation _idleAnim;
    public Animation _fallAnim;
    public Animation _attackAnim1;
    public Animation _attackAnim2;

    private Vector2 AnimatedSpriteRenderingPosition;
    public int AttackColliderOffset;

    public Area2D DamageArea;


    public PlayerInfo PlayerInfo = new();

    private PlayerUI Screen;
    private Pausemenu pauseMenu;
    public AnimatedSprite AnimatedSprite;
    private StateController _stateController;

    public Player() : base(4) { }

    public override void Load()
    {
        Engine.MainCharacter = this;
        
        Screen = new PlayerUI();
        pauseMenu = new Pausemenu(this);

        _atlas = TextureAtlas.FromFile(Engine.Content, "Assets/Atlas/Player/player-atlas.xml", "Assets/Animations/Player/PlayerModel3Atlas");

        _runAnim = _atlas.GetAnimation("run-animation");
        _idleAnim = _atlas.GetAnimation("idle-animation");
        _fallAnim = _atlas.GetAnimation("fall-animation");
        _attackAnim1 = _atlas.GetAnimation("attack-animation-1");
        _attackAnim2 = _atlas.GetAnimation("attack-animation-2");

        AnimatedSprite = _atlas.CreateAnimatedSprite("idle-animation");
        AnimatedSprite.LayerDepth = 0.5f;


        KinematicBase.Collider = new Area2D(new Rectangle(400, 150, 10, 25), true, this);

        KinematicBase.Position = Position;

        Circle attackCircle = new(0, 0, 30);
        DamageArea = new Area2D(attackCircle, false, this);

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

        DamageArea.Circ.X = KinematicBase.Collider.Rect.X + AttackColliderOffset;
        DamageArea.Circ.Y = KinematicBase.Collider.Rect.Y - 10;

        ApplyGravity();

        _stateController.Update(gameTime);

        KinematicBase.UpdateCollider(gameTime);
        SaveManager.PlayerData.CurrentPosition = KinematicBase.Position;

        FlipSprite();

        AnimatedSprite.Position = new Vector2(KinematicBase.Collider.Rect.X - 64 + PlayerInfo.textureOffset, KinematicBase.Collider.Rect.Y - 55);
        AnimatedSprite.Update(gameTime);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {   
        Engine.DrawManager.Draw(AnimatedSprite);
        DrawHelper.DrawRectangle(KinematicBase.Collider.Rect, Color.Red, 2);
        DrawHelper.DrawString(_stateController.CurrentState.ToString(), Color.White, CCamera.CurrentCamera.GetScreenEdges().TopLeft);
    }

    public void ApplyGravity()
    {
        if (!KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = MathF.Min(
                KinematicBase.Velocity.Y + PlayerInfo.Gravity * Engine.DeltaTime,
                PlayerInfo.TerminalVelocity
            );
        }
        else if (KinematicBase.Velocity.Y > 0)
        {
            KinematicBase.Velocity.Y = 0;
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
        KinematicBase.Velocity.X = MoveToward(KinematicBase.Velocity.X, targetSpeed, accel * Engine.DeltaTime);
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
