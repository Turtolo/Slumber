using ConstructEngine;
using ConstructEngine.Components.Entity;
using ConstructEngine.Components.Physics;
using ConstructEngine.Graphics;
using ConstructEngine.Area;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slumber.Logic;
using Slumber.Screens;
using System;
using ConstructEngine.Helpers;
using ConstructEngine.Util;
using System.Security.Cryptography;

namespace Slumber.Entities;

public class Player : Entity, Entity.IEntity
{
    private TextureAtlas _atlas;

    public Animation _runAnim;
    public Animation _idleAnim;
    public Animation _fallAnim;
    public Animation _attackAnim1;
    public Animation _attackAnim2;

    private Vector2 AnimatedSpriteRenderingPosition;

    public Keys MoveRightKey = Keys.Right;
    public Keys MoveLeftKey = Keys.Left;
    public Keys JumpKey = Keys.Z;
    public Keys AttackKey = Keys.X;

    public int AttackColliderOffset;

    public Area2D DamageArea;
    public HealthComponent HealthComponent;

    public PlayerInfo PlayerInfo = new();

    private PlayerUI Screen;
    private Pausemenu pauseMenu;

    private StateController _stateController;

    public Player() : base(4) { }

    public override void Load()
    {
        Screen = new PlayerUI();
        pauseMenu = new Pausemenu();

        _atlas = TextureAtlas.FromFile(Core.Content, "Assets/Atlas/Player/player-atlas.xml", "Assets/Animations/Player/PlayerModel3Atlas");

        _runAnim = _atlas.GetAnimation("run-animation");
        _idleAnim = _atlas.GetAnimation("idle-animation");
        _fallAnim = _atlas.GetAnimation("fall-animation");
        _attackAnim1 = _atlas.GetAnimation("attack-animation-1");
        _attackAnim2 = _atlas.GetAnimation("attack-animation-2");

        AnimatedSprite = _atlas.CreateAnimatedSprite("idle-animation");
        AnimatedSprite.LayerDepth = 0.5f;

        KinematicBase.Collider = new Area2D(new Rectangle(400, 150, 10, 25), true, this);

        Circle attackCircle = new(0, 0, 30);
        DamageArea = new Area2D(attackCircle, false, this);

        HealthComponent = new HealthComponent(this, 5, KinematicBase.Collider, this);

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
        

        _stateController = new StateController(idle, new IState[]
        {
            grounded, idle, run, attack, jump, fall, wallSlide, wallJump
        });
    }

    public void Update(GameTime gameTime)
    {
        HealthComponent.Update(gameTime);

        DamageArea.Circ.X = KinematicBase.Collider.Rect.X + AttackColliderOffset;
        DamageArea.Circ.Y = KinematicBase.Collider.Rect.Y - 10;

        ApplyGravity();

        _stateController.Update(gameTime);

        KinematicBase.UpdateCollider(gameTime);
        SaveManager.PlayerData.CurrentPosition = KinematicBase.Position;

        FlipSprite();

        AnimatedSpriteRenderingPosition = new Vector2(KinematicBase.Collider.Rect.X - 64, KinematicBase.Collider.Rect.Y - 55);
        AnimatedSprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawSprites(spriteBatch, AnimatedSpriteRenderingPosition, PlayerInfo.textureOffset);
        DrawHelper.DrawString(_stateController.CurrentState.ToString(), Color.White, Camera.CurrentCamera.GetScreenEdges().TopLeft);
    }

    public void ApplyGravity()
    {
        if (!KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = MathF.Min(
                KinematicBase.Velocity.Y + PlayerInfo.Gravity * Core.DeltaTime,
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

        if (Core.Input.Keyboard.IsKeyDown(MoveLeftKey) && !Core.Input.Keyboard.IsKeyDown(MoveRightKey))
        {
            targetSpeed = -PlayerInfo.MoveSpeed;
            PlayerInfo.dir = -1;
        }
        else if (Core.Input.Keyboard.IsKeyDown(MoveRightKey) && !Core.Input.Keyboard.IsKeyDown(MoveLeftKey))
        {
            targetSpeed = PlayerInfo.MoveSpeed;
            PlayerInfo.dir = 1;
        }

        float accel = (MathF.Abs(targetSpeed) > 0) ? PlayerInfo.Acceleration : PlayerInfo.Deceleration;
        KinematicBase.Velocity.X = MoveToward(KinematicBase.Velocity.X, targetSpeed, accel * Core.DeltaTime);
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
