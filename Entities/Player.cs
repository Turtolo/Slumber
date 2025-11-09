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
using Timer = ConstructEngine.Util.Timer;
using System;
using ConstructEngine.Helpers;

namespace Slumber.Entities;

public class Player : Entity, Entity.IEntity
{
    private TextureAtlas _atlas;
    private TextureAtlas _atlasFeet;


    Animation _runAnim;
    Animation _idleAnim;
    Animation _fallAnim;
    Animation _attackAnim1;
    Animation _attackAnim2;

    private Vector2 AnimatedSpriteRenderingPosition;

    Keys MoveRightKey = Keys.Right;
    Keys MoveLeftKey = Keys.Left;
    Keys JumpKey = Keys.Z;
    Keys AttackKey = Keys.X;

    int AttackColliderOffset;

    Area2D DamageArea;

    HealthComponent HealthComponent;

    PlayerInfo PlayerInfo = new();

    private PlayerUI Screen;
    private Pausemenu pauseMenu;

    public Player() : base(4)
    {
    }

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


        Circle AttackCircle = new(0, 0, 30);


        DamageArea = new Area2D(AttackCircle, false, this);

        HealthComponent = new HealthComponent(this, 5, KinematicBase.Collider, this);


    }


    public void Update(GameTime gameTime)
    {
        HealthComponent.Update(gameTime);
        DamageArea.Circ.X = KinematicBase.Collider.Rect.X + AttackColliderOffset;
        DamageArea.Circ.Y = KinematicBase.Collider.Rect.Y - 10;



        ApplyGravity();
        HandleHorizontalInput();
        HandleJump();
        HandleWall();
        HandleWallJump();

        KinematicBase.UpdateCollider(gameTime);
        SaveManager.PlayerData.CurrentPosition = KinematicBase.Position;


        FlipSprite();
        Animation();

        AnimatedSpriteRenderingPosition = new Vector2(KinematicBase.Collider.Rect.X - 64, KinematicBase.Collider.Rect.Y - 55);

        AnimatedSprite.Update(gameTime);

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.K)) SaveManager.SaveData();
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.L)) SaveManager.LoadData();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawSprites(spriteBatch, AnimatedSpriteRenderingPosition, PlayerInfo.textureOffset);
        DrawHelper.DrawRectangle(KinematicBase.Collider.Rect, Color.Red, 2);
    }

    

    //Functions
    private void HandleWall()
    {
        if (KinematicBase.IsOnWall())
        {
            PlayerInfo.wallSlide = true;
        }

        if (!KinematicBase.IsOnWall() || KinematicBase.IsOnGround() || KinematicBase.Velocity.Y < 0)
        {
            PlayerInfo.wallSlide = false;
        }

        if (PlayerInfo.wallSlide)
        {
            KinematicBase.Velocity.Y = 0;
        }
    }

    private void HandleWallJump()
    {
        if (KinematicBase.IsOnWall() && !KinematicBase.IsOnGround() && PlayerInfo.wallSlide)
        {
            if (Core.Input.Keyboard.WasKeyJustPressed(JumpKey))
            {
                PlayerInfo.canMove = false;

                if (PlayerInfo.dir)
                {
                    KinematicBase.Velocity.X = -PlayerInfo.WallJumpHorizontalSpeed;
                }
                else
                {
                    KinematicBase.Velocity.X = PlayerInfo.WallJumpHorizontalSpeed;
                }

                KinematicBase.Velocity.Y = -PlayerInfo.WallJumpVerticalSpeed;

                Timer.Wait(0.12f, () => { PlayerInfo.canMove = true; });
            }
        }
    }

    private void ApplyGravity()
    {
        if (!KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = MathF.Min(KinematicBase.Velocity.Y + PlayerInfo.Gravity * Core.DeltaTime, PlayerInfo.TerminalVelocity);
        }
        else if (KinematicBase.Velocity.Y > 0)
        {
            KinematicBase.Velocity.Y = 0;
        }
    }


    private void HandleHorizontalInput()
    {
        float targetSpeed = 0f;

        if (Core.Input.Keyboard.IsKeyDown(MoveLeftKey) && !Core.Input.Keyboard.IsKeyDown(MoveRightKey))
        {
            targetSpeed = -PlayerInfo.MoveSpeed;
            PlayerInfo.dir = false;
        }
        else if (Core.Input.Keyboard.IsKeyDown(MoveRightKey) && !Core.Input.Keyboard.IsKeyDown(MoveLeftKey))
        {
            targetSpeed = PlayerInfo.MoveSpeed;
            PlayerInfo.dir = true;
        }

        float accel = (MathF.Abs(targetSpeed) > 0) ? PlayerInfo.Acceleration : PlayerInfo.Deceleration;
        KinematicBase.Velocity.X = MoveToward(KinematicBase.Velocity.X, targetSpeed, accel * Core.DeltaTime);
    }

    private float MoveToward(float current, float target, float maxDelta)
    {
        if (MathF.Abs(target - current) <= maxDelta) return target;
        return current + MathF.Sign(target - current) * maxDelta;
    }


    private void HandleAttack()
    {

        if (PlayerInfo.attackBufferTimer > 0)
            PlayerInfo.attackBufferTimer -= Core.DeltaTime;

        if (Core.Input.Keyboard.WasKeyJustPressed(AttackKey))
        {
            PlayerInfo.attackBufferTimer = PlayerInfo.attackBufferTime;
        }

        if (!PlayerInfo.attacking)
        {
            if (PlayerInfo.attackBufferTimer > 0)
            {
                StartAttack();
                PlayerInfo.attackBufferTimer = 0;
            }
        }
        else
        {
            if (AnimatedSprite.finished)
            {
                PlayerInfo.attacking = false;
                DamageArea.Enabled = false;
            }
        }
    }

    private void StartAttack()
    {
        PlayerInfo.AttackCount++;
        PlayerInfo.attacking = true;
        DamageArea.Enabled = true;
    }

    private void HandleJump()
    {
        if ((Core.Input.Keyboard.WasKeyJustPressed(JumpKey) || PlayerInfo.bufferActivated) && KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = PlayerInfo.JumpForce;
            PlayerInfo.bufferActivated = false;
        }

        if (PlayerInfo.VariableJump && Core.Input.Keyboard.WasKeyJustReleased(JumpKey) && KinematicBase.Velocity.Y < 0)
        {
            KinematicBase.Velocity.Y *= 0.5f;
        }
    }


    private void FlipSprite()
    {
        if (PlayerInfo.dir)
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

    private void Animation()
    {
        if (!PlayerInfo.attacking)
        {
            if (KinematicBase.IsOnGround())
            {
                if (KinematicBase.Velocity != Vector2.Zero && !KinematicBase.IsOnWall())
                {
                    AnimatedSprite.PlayAnimation(_runAnim, false);
                }
                else
                {
                    AnimatedSprite.PlayAnimation(_idleAnim, false);
                }
            }

            else
            {
                AnimatedSprite.PlayAnimation(_fallAnim, false);
            }
        }

        else
        {
            if (PlayerInfo.AttackCount == 1) AnimatedSprite.PlayAnimation(_attackAnim1, false);
            if (PlayerInfo.AttackCount == 2) AnimatedSprite.PlayAnimation(_attackAnim2, false);
        }
    }
}
