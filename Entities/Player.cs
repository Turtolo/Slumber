using System;
using System.ComponentModel;
using System.Net.Mime;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Timers;
using ConstructEngine;
using ConstructEngine.Components.Entity;
using ConstructEngine.Components.Physics;
using ConstructEngine.Directory;
using ConstructEngine.Graphics;
using ConstructEngine.Gum;
using ConstructEngine.Physics;
using ConstructEngine.Util;
using Gum.Forms.Controls;
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

    string FileSavePath = "PlayerData.json";

    private Vector2 AnimatedSpriteRenderingPosition;

    Keys MoveRightKey = Keys.Right;
    Keys MoveLeftKey = Keys.Left;
    Keys JumpKey = Keys.Z;
    Keys AttackKey = Keys.X;

    int AttackColliderOffset;

    Collider DamageArea;

    HealthComponent HealthComponent;

    PlayerInfo PlayerInfo = new();
    PlayerData PlayerData = new();

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
                
        _runAnim = _atlas.CreateAnimatedSprite("run-animation").Animation;
        _idleAnim = _atlas.CreateAnimatedSprite("idle-animation").Animation;
        _fallAnim = _atlas.CreateAnimatedSprite("fall-animation").Animation;
    

        AnimatedSprite = _atlas.CreateAnimatedSprite("idle-animation");
        AnimatedSprite.LayerDepth = 0.5f;

        KinematicBase.Collider = new Collider(new Rectangle(400, 150, 10, 25), true, this);


        Circle AttackCircle = new(0, 0, 30);


        DamageArea = new Collider(AttackCircle, false, this);

        HealthComponent = new HealthComponent(this, 5, KinematicBase.Collider);

    
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

    


        HealthComponent.Update(gameTime);

        DamageArea.Circ.X = KinematicBase.Collider.Rect.X + AttackColliderOffset;
        DamageArea.Circ.Y = KinematicBase.Collider.Rect.Y - 10;

        
        Screen.LabelInstance.Text = "Health: " + HealthComponent.CurrentHealth + " Colliding: " + KinematicBase.Collider.IsIntersectingAny() + " Type: "  + KinematicBase.Collider.GetCurrentlyIntersectingCollider()?.GetType();
        
        HandleInput(gameTime);

        HandleGravity(gameTime);
        
        KinematicBase.UpdateCollider(gameTime);
            
        if (KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = 0;
        }
        
        AnimatedSpriteRenderingPosition = new Vector2(KinematicBase.Collider.Rect.X - 45, KinematicBase.Collider.Rect.Y - 39);
        
        FlipSprite();
        HandleWall();
        HandleWallJump();
        Animation();

        AnimatedSprite.Update(gameTime);
        
        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.K))
        {
            SaveData();
        }

        if (Core.Input.Keyboard.WasKeyJustPressed(Keys.L))
        {
            LoadDataFromSave();
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawSprites(spriteBatch, AnimatedSpriteRenderingPosition, PlayerInfo.textureOffset);

        //ColliderDraw.DrawCircle(AttackCollider.Circ, Color.Red, 2);
        //DrawHelper.DrawRectangle(KinematicBase.Collider.Rect, Color.Red, 2, 0.6f);
    }
    
    private void SaveData()
    {
        PlayerData.CurrentPosition = KinematicBase.Collider.GetPosition();
        PlayerData.CurrentHealth = HealthComponent.CurrentHealth;

        FileSaver.SaveDataToJson(PlayerData, "", FileSavePath);
    }

    private void LoadDataFromSave()
    {
        FileSaver.LoadDataFromJson(PlayerData, FileSavePath);

        KinematicBase.Collider.Rect.X = (int)PlayerData.CurrentPosition.X;
        KinematicBase.Collider.Rect.Y = (int)PlayerData.CurrentPosition.Y;
        HealthComponent.CurrentHealth = PlayerData.CurrentHealth;
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
    }
}
