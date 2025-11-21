
namespace Slumber.Entities;

public class Enemy : KinematicEntity, IKinematicEntity
{
    public float Gravity = 1300f;
    private int Health;
    private int Direction = 1;
    private int TextureOffset;

    private RayCast2D EnemyRay;
    private RayCast2D EnemyRayNotDown;

    private Vector2 SpritePosition;
    Vector2 RayPos;


    private bool CanTakeDamage = true;

    private TextureAtlas Atlas;
    private Texture2D AltasTexture;

    private Animation RunAnimation;
    AnimatedSprite AnimatedSprite;

    private Area2D TakeDamageArea;

    public Enemy() : base(1)
    {

    }


    public override void Load()
    {
        Atlas = TextureAtlas.FromFile(Engine.Content, "Assets/Atlas/enemyatlas.xml", "Assets/Animations/Enemies/grassspidersheet");

        RunAnimation = Atlas.CreateAnimatedSprite("run-animation").Animation;

        AnimatedSprite = Atlas.CreateAnimatedSprite("run-animation");
        AnimatedSprite.LayerDepth = 0.5f;

        KinematicBase.Collider = new RectangleShape2D(400, 150, 16, 16);

        Health = 5;

        float forwardOffset = KinematicBase.Collider.BoundingBox.Width / 2f + 5f;

        RayPos = new Vector2(
            KinematicBase.Collider.BoundingBox.Center.X + forwardOffset * Direction,
            KinematicBase.Collider.BoundingBox.Bottom
        );

        EnemyRay = new RayCast2D(RayPos, 90, 50);
        EnemyRayNotDown = new RayCast2D(RayPos, 0, 5);

        TakeDamageArea = new Area2D(KinematicBase.Collider);
    }

    public override void Update(GameTime gameTime)
    {
        float forwardOffset = KinematicBase.Collider.BoundingBox.Width / 2f + 5f;

        RayPos.X = KinematicBase.Collider.BoundingBox.Center.X + forwardOffset * Direction;
        RayPos.Y = KinematicBase.Collider.BoundingBox.Bottom;

        EnemyRay.Update(RayPos, 90, 50);
        EnemyRayNotDown.Update(RayPos, 0, 5);

        EnemyRay.CheckIntersection(Node.AllInstances.OfType<RegionNode>());
        EnemyRayNotDown.CheckIntersection(Node.AllInstances.OfType<RegionNode>());

        //HandleDamage();

        HandleGravity();

        HandleMovement();

        FlipSprite(Direction);

        KinematicBase.UpdateCollider(gameTime);

        if (KinematicBase.IsOnGround())
        {
            KinematicBase.Velocity.Y = 0;
        }

        SpritePosition = new Vector2(KinematicBase.Collider.BoundingBox.X, KinematicBase.Collider.BoundingBox.Y);

        AnimatedSprite.PlayAnimation(RunAnimation, true);

        AnimatedSprite.Update(gameTime);

        AnimatedSprite.Position = new Vector2(SpritePosition.X + TextureOffset, SpritePosition.Y);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Engine.DrawManager.Draw(AnimatedSprite);
        DrawHelper.DrawRay(EnemyRay, Color.Red, 2);
    }

    private void FlipSprite(int direction)
    {
        if (direction == 1)
        {
            if (AnimatedSprite != null) AnimatedSprite.Effects = SpriteEffects.None;        }
        else
        {
            if (AnimatedSprite != null) AnimatedSprite.Effects = SpriteEffects.FlipHorizontally;
        }
    }

    private void HandleGravity()
    {
        KinematicBase.Velocity.Y += Gravity * Engine.DeltaTime;
    }

    private void HandleMovement()
    {

        if (!EnemyRay.HasHit || EnemyRayNotDown.HasHit)
        {
            Direction = -Direction;
        }

        KinematicBase.Velocity.X = 80 * Direction;
    }

    public void TakeDamage(int DamageAmount)
    {
        if (!CanTakeDamage) return;
        CanTakeDamage = false;
        Health -= DamageAmount;

        CTimer.Wait(0.7f, () => { CanTakeDamage = true; });
    }

}
