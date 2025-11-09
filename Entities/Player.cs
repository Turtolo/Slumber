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
using System.Runtime.CompilerServices;

namespace Slumber.Entities;

public class Player : Entity, Entity.IEntity
{
    private TextureAtlas _atlas;
    private TextureAtlas _atlasFeet;

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

    public StateController StateMachine;

    private Pausemenu pauseMenu;

    public Player() : base(4)
    {

    }

    public override void Load()
    {
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

        var idleState = new IdleState(this);
        var runState = new RunState(this);
        var jumpState = new JumpState(this);
        var halfJumpState = new HalfJumpState(this);
        var fallState = new FallState(this);
        var fallMoveState = new FallMoveState(this);
        var takeDamageState = new TakeDamageState(this);

        StateMachine = new(idleState, [idleState, runState, jumpState, halfJumpState, fallState, fallMoveState, takeDamageState]);

        DamageArea = new Area2D(AttackCircle, false, this);

        HealthComponent = new HealthComponent(this, 5, KinematicBase.Collider, this);


    }


    public void Update(GameTime gameTime)
    {
        HealthComponent.Update(gameTime);
        DamageArea.Circ.X = KinematicBase.Collider.Rect.X + AttackColliderOffset;
        DamageArea.Circ.Y = KinematicBase.Collider.Rect.Y - 10;

        StateMachine.Update(gameTime);
        Console.WriteLine(StateMachine.CurrentState);

        HandleWall();
        HandleWallJump();

        KinematicBase.UpdateCollider(gameTime);
        SaveManager.PlayerData.CurrentPosition = KinematicBase.Position;

        if (HealthComponent.TakingDamage)
        {
            StateMachine.RequestTransition("TakeDamageState");
        }


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
        DrawHelper.DrawString(StateMachine.CurrentState.ToString(), Color.White, Camera.CurrentCamera.GetScreenEdges().TopLeft);
        //DrawHelper.DrawRectangle(KinematicBase.Collider.Rect, Color.Red, 2);
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

                if (PlayerInfo.dir == 1)
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

    public void ApplyGravity()
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




    public float MoveToward(float current, float target, float maxDelta)
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




    private void FlipSprite()
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
