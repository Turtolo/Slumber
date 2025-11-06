using ConstructEngine;
using ConstructEngine.Components.Entity;
using ConstructEngine.Components.Physics;
using ConstructEngine.Graphics;
using ConstructEngine.Area;
using ConstructEngine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Slumber.Logic;
using Slumber.Screens;
using Timer = ConstructEngine.Util.Timer;

namespace Slumber.Entities;

public class Player : Entity, Entity.IEntity
{
    private TextureAtlas _atlas;
    private TextureAtlas _atlasFeet;


    Animation _runAnim;
    Animation _idleAnim;
    Animation _fallAnim;
    Animation _attackAnim;

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
        _attackAnim = _atlas.GetAnimation("attack-animation");

        AnimatedSprite = _atlas.CreateAnimatedSprite("idle-animation");
        AnimatedSprite.LayerDepth = 0.5f;

        KinematicBase.Collider = new Area2D(new Rectangle(400, 150, 10, 25), true, this);


        Circle AttackCircle = new(0, 0, 30);


        DamageArea = new Area2D(AttackCircle, false, this);

        HealthComponent = new HealthComponent(this, 5, KinematicBase.Collider, this);


    }

    public void Update(GameTime gameTime)
    {
        if (KinematicBase.Velocity.Y < 0)
        {
            PlayerInfo.falling = false;
        }
        else
        {
            PlayerInfo.falling = true;
        }


        SaveManager.PlayerData.CurrentPosition = KinematicBase.Position;

        HealthComponent.Update(gameTime);

        DamageArea.Circ.X = KinematicBase.Collider.Rect.X + AttackColliderOffset;
        DamageArea.Circ.Y = KinematicBase.Collider.Rect.Y - 10;


        Screen.LabelInstance.Text = "Health: " + HealthComponent.CurrentHealth + " Colliding: " + KinematicBase.Collider.IsIntersectingAny() + " Type: " + KinematicBase.Collider.GetCurrentlyIntersectingArea()?.GetType();

        HandleInput(gameTime);

        HandleGravity(gameTime);

        KinematicBase.UpdateCollider(gameTime);

        if (KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = 0;
        }

        AnimatedSpriteRenderingPosition = new Vector2(KinematicBase.Collider.Rect.X - 64, KinematicBase.Collider.Rect.Y - 55);

        FlipSprite();
        HandleWall();
        HandleWallJump();
        HandleAttack();
        Animation();

        AnimatedSprite.Update(gameTime);

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.K))
        {
            SaveManager.SaveData();
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.L))
        {
            SaveManager.LoadData();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawSprites(spriteBatch, AnimatedSpriteRenderingPosition, PlayerInfo.textureOffset);


        DrawHelper.DrawString(HealthComponent.CurrentHealth.ToString(), Color.Red, Camera.CurrentCamera.GetScreenEdges().TopLeft);
        DrawHelper.DrawCircle(DamageArea.Circ, Color.Red, 2);
        DrawHelper.DrawRectangle(KinematicBase.Collider.Rect, Color.Red, 2, 0.6f);
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

    private void HandleGravity(GameTime gameTime)
    {
        KinematicBase.Velocity.Y += PlayerInfo.Gravity * Core.DeltaTime;
    }

    private void HandleInput(GameTime gameTime)
    {
        if (PlayerInfo.canMove)
        {
            KinematicBase.Velocity.X = 0;

            if (Core.Input.Keyboard.IsKeyDown(MoveLeftKey) && !Core.Input.Keyboard.IsKeyDown(MoveRightKey))
            {
                KinematicBase.Velocity.X = -PlayerInfo.MoveSpeed;
                PlayerInfo.dir = false;
            }

            if (Core.Input.Keyboard.IsKeyDown(MoveRightKey) && !Core.Input.Keyboard.IsKeyDown(MoveLeftKey))
            {
                KinematicBase.Velocity.X = PlayerInfo.MoveSpeed;
                PlayerInfo.dir = true;
            }
        }

        //HandleAttack();

        if (PlayerInfo.AttackCount == 3)
        {
            PlayerInfo.AttackCount = 0;
        }

        Jump(gameTime);
    }

    private void HandleAttack()
    {
        if (!PlayerInfo.attacking)
        {
            if (Core.Input.Keyboard.WasKeyJustPressed(AttackKey))
            {
                PlayerInfo.AttackCount++;

                PlayerInfo.attacking = true;

                DamageArea.Enabled = true;

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

    private void Jump(GameTime gameTime)
    {
        if (Core.Input.Keyboard.WasKeyJustPressed(JumpKey) || PlayerInfo.bufferActivated)
        {
            if (KinematicBase.IsOnGround())
            {
                PlayerInfo._upwardAir = true;
                PlayerInfo.bufferActivated = false;
                KinematicBase.Velocity.Y = PlayerInfo.JumpForce;
            }
            else
            {
                PlayerInfo.bufferActivated = true;
                Timer.Wait(PlayerInfo.bufferTimer, () => PlayerInfo.bufferActivated = false);
            }
        }

        if (Core.Input.Keyboard.WasKeyJustReleased(JumpKey) && KinematicBase.Velocity.Y < 0)
        {
            KinematicBase.Velocity.Y = PlayerInfo.JumpForce / 4;
        }

        if (PlayerInfo._upwardAir && KinematicBase.Velocity.Y > 0)
        {
            PlayerInfo._upwardAir = false;
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
            PlayerInfo.textureOffset = 4;
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
            AnimatedSprite.PlayAnimation(_attackAnim, false);
        }
    }
}
