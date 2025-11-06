
using System;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using ConstructEngine;
using ConstructEngine.Components.Entity;
using ConstructEngine.Graphics;
using ConstructEngine.Objects;
using ConstructEngine.Area;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Timer = ConstructEngine.Util.Timer;
using ConstructEngine.Helpers;
using System.Net.Http.Headers;

namespace Slumber.Entities;

public class Enemy : Entity, Entity.IEntity
{
    public float Gravity = 1300f;

    private int Health;
    private int Direction = 1;
    private int TextureOffset;

    private Ray2D EnemyRay;
    private Ray2D EnemyRayNotDown;

    private Vector2 SpritePosition;

    private bool CanTakeDamage = true;
    private bool RayColliding;
    private bool RayCollidingNotDown;

    private TextureAtlas Atlas;

    private Texture2D AltasTexture;

    private Animation RunAnimation;

    private Area2D TakeDamageArea;

    Vector2 RayPos;

    public Enemy() : base(1)
    {

    }


    public override void Load()
    {
        Atlas = TextureAtlas.FromFile(Core.Content, "Assets/Atlas/enemyatlas.xml", "Assets/Animations/Enemies/grassspidersheet");

        RunAnimation = Atlas.CreateAnimatedSprite("run-animation").Animation;

        AnimatedSprite = Atlas.CreateAnimatedSprite("run-animation");
        AnimatedSprite.LayerDepth = 0.5f;

        KinematicBase.Collider = new Area2D(new Rectangle(400, 150, 16, 16), true, this);

        Health = 5;

        float forwardOffset = KinematicBase.Collider.Rect.Width / 2f + 5f;

        RayPos = new Vector2(
            KinematicBase.Collider.Rect.Center.X + forwardOffset * Direction,
            KinematicBase.Collider.Rect.Bottom
        );

        EnemyRay = new Ray2D(RayPos, 90, 50);
        EnemyRayNotDown = new Ray2D(RayPos, 0, 5);

        TakeDamageArea = KinematicBase.Collider;
    }

    public void Update(GameTime gameTime)
    {
        float forwardOffset = KinematicBase.Collider.Rect.Width / 2f + 5f;

        RayPos.X = KinematicBase.Collider.Rect.Center.X + forwardOffset * Direction;
        RayPos.Y = KinematicBase.Collider.Rect.Bottom;

        EnemyRay.Update(RayPos, 90, 50);
        EnemyRayNotDown.Update(RayPos, 0, 5);

        EnemyRay.CheckIntersection(Area2D.AreaList, typeof(CollisionObject));
        EnemyRayNotDown.CheckIntersection(Area2D.AreaList, typeof(CollisionObject));

        HandleDamage();

        HandleGravity();

        HandleMovement();

        FlipSprite(Direction);

        if (Health <= 0)
        {
            Free();
        }

        KinematicBase.UpdateCollider(gameTime);

        if (KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = 0;
        }

        SpritePosition = new Vector2(KinematicBase.Collider.Rect.X, KinematicBase.Collider.Rect.Y);

        AnimatedSprite.PlayAnimation(RunAnimation, true);

        AnimatedSprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawSprites(spriteBatch, SpritePosition, TextureOffset);

        //DrawHelper.DrawString(Health.ToString(), Color.DarkBlue, new Vector2(KinematicBase.Position.X, KinematicBase.Position.Y - 10));
    }

    private void FlipSprite(int direction)
    {
        if (direction == 1)
        {
            if (AnimatedSprite != null) AnimatedSprite.Effects = SpriteEffects.None;
            if (AnimatedSpriteFeet != null) AnimatedSpriteFeet.Effects = SpriteEffects.None;
        }
        else
        {
            if (AnimatedSprite != null) AnimatedSprite.Effects = SpriteEffects.FlipHorizontally;
            if (AnimatedSpriteFeet != null) AnimatedSpriteFeet.Effects = SpriteEffects.FlipHorizontally;
        }
    }

    private void HandleGravity()
    {
        KinematicBase.Velocity.Y += Gravity * Core.DeltaTime;
    }

    private void HandleMovement()
    {

        if (!EnemyRay.HasHit || EnemyRayNotDown.HasHit)
        {
            Direction = -Direction;
        }

        KinematicBase.Velocity.X = 80 * Direction;
    }

    private void HandleDamage()
    {
        if (KinematicBase.Collider.IsIntersectingAny())
        {

            Entity OtherEntity = (Entity)KinematicBase.Collider.GetCurrentlyIntersectingArea().Root;
            
            if (OtherEntity.GetType() != typeof(Enemy))
                TakeDamage(OtherEntity.DamageAmount);
        }

    }


    public void TakeDamage(int DamageAmount)
    {
        if (!CanTakeDamage) return;
        CanTakeDamage = false;
        Health -= DamageAmount;

        Timer.Wait(0.7f, () => { CanTakeDamage = true; });
    }

}
