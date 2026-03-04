using System.Net.Mail;

namespace Slumber
{
    public class Player : KinematicBody2D
    {
        #region Configuration

        public float MoveSpeed = 100f;
        public float Acceleration = 3500f;
        public float Deceleration = 2500f;

        public float Gravity = 1300f;
        public float TerminalVelocity = 1200f;
        public float JumpForce = -350;

        public float WallSlideGravity = 20f;
        public float WallJumpHorizontalSpeed = 200f;
        public float WallJumpVerticalSpeed = 300f;

        public float CoyoteTime = 0.12f;
        public float JumpBufferTime = 0.2f;
        public float AttackBufferTime = 0.2f;

        public bool AllowControl = true;

        #endregion

        #region State

        public Vector2 PlayerAxis;
        public int PlayerDirection;

        private bool jumpReleased = false;
        private bool wallSlideTriggered = false;
        private bool jumpBuffered = false;
        private bool canCoyoteJump = false;
        private bool wasOnFloor = false;

        private int attackCounter;
        private bool attackBuffer;
        private bool isAttacking = false;

        #endregion

        #region Components

        public AnimatedSprite2D Sprite;
        public AnimatedSprite2D FeetSprite;

        public Area2D AttackArea;

        #endregion

        #region Constructors

        public Player() {}

        public override void OnEnter()
        {
            base.OnEnter();

            var c = Engine.Tree.Create<CollisionShape2D>();
            c.Shape = new RectangleShape2D(10, 25);

            c.SetParent(this);


            var animations = AsepriteLoader.LoadAnimations(
                new("Assets/Animations/PlayerModel3Atlas"),
                PathHelper.Combine("Raw/Raw/PlayerModel3.json")
            );

            var feetAnimations = AsepriteLoader.LoadAnimations(
                new("Assets/Animations/PlayerModel3AtlasFeet"),
                PathHelper.Combine("Raw/Raw/PlayerModel3.json")
            );

            FeetSprite = Engine.Tree.Create<AnimatedSprite2D>().SetProperties(n =>
            {
                n.SetParent(this);
                n.Atlas = feetAnimations;
                n.LocalPosition = new Vector2(6, 9);
                n.IsLooping = true;
            });

            Sprite = Engine.Tree.Create<AnimatedSprite2D>().SetProperties(n =>
            {
                n.SetParent(this);
                n.Atlas = animations;
                n.LocalPosition = new Vector2(6, 9);
                n.IsLooping = true;
            });

            LocalDepth = 5;

            AttackArea = Engine.Tree.Create<Area2D>().SetProperties(n =>
            {
                n.AddChild(Engine.Tree.Create<CollisionShape2D>().SetProperties(c =>
                {
                    c.Shape = new CircleShape2D(30);
                    c.Disabled = true;
                }));
                n.SetParent(this);
                n.LocalPosition = new Vector2(50, 5);
            });
        }

        #endregion

        #region Update

        public override void PhysicsUpdate(float delta)
        {
            PlayerAxis = Engine.Input.GetAxis("MoveLeft", "MoveRight", "MoveDown", "MoveUp");
            PlayerDirection = (int)PlayerAxis.X != 0 ? (int)PlayerAxis.X : PlayerDirection;

            HandleCoyoteTime();
            HandleJump();
            HandleMovementInput();
            HandleWallSlide();
            HandleDeceleration(delta);
            HandleAttack();
            ApplyGravity(delta);
            
            base.PhysicsUpdate(delta);
        }

        public override void ProcessUpdate(float delta)
        {
            base.ProcessUpdate(delta);

            AnimateSprite();
            FlipSprite();

            if (attackCounter == 2)
                attackCounter = 0;
        }

        public override void SubmitCall()
        {
            base.SubmitCall(); 
            //CollisionShape.Shape.Draw();
        }

        #endregion

        #region Movement

        public void HandleMovementInput()
        {
            if (!AllowControl)
                return;

            float targetSpeed = MoveSpeed * PlayerAxis.X;

            if (targetSpeed != 0)
                Velocity.X = MoveToward(Velocity.X, targetSpeed, Acceleration);
        }

        public void HandleDeceleration(float delta)
        {
            Velocity.X = PlayerAxis.X == 0 ? MoveToward(Velocity.X, 0, Deceleration * delta) : Velocity.X;
        }

        public float MoveToward(float current, float target, float maxDelta)
        {
            if (MathF.Abs(target - current) <= maxDelta)
                return target;

            return current + MathF.Sign(target - current) * maxDelta;
        }

        #endregion

        #region Jumping and Gravity

        public void ApplyGravity(float delta)
        {
            //if (wallSlideTriggered)
                //return;

            if (!IsOnFloor)
            {
                Velocity.Y = MathF.Min(
                    Velocity.Y + Gravity * delta,
                    TerminalVelocity
                );
            }
            else if (Velocity.Y > 0)
            {
                Velocity.Y = 0;
            }
        }

        public void HandleJump()
        {
            if (IsOnFloor || canCoyoteJump)
            {
                if (Engine.Input.IsActionJustPressed("Jump") || jumpBuffered)
                {
                    Velocity.Y = JumpForce;
                    jumpReleased = false;
                    canCoyoteJump = false;
                    jumpBuffered = false;
                }
            }
            else
            {
                if (Engine.Input.IsActionJustPressed("Jump"))
                {
                    jumpBuffered = true;
                    Engine.Timer.Wait(JumpBufferTime, () => jumpBuffered = false);
                }
            }

            if (!jumpReleased && Engine.Input.IsActionJustReleased("Jump") && Velocity.Y < 0)
            {
                Velocity.Y /= 2f;
                jumpReleased = true;
            }
        }

        private void HandleCoyoteTime()
        {
            if (wasOnFloor && !IsOnFloor && Velocity.Y >= 0f)
            {
                canCoyoteJump = true;
                Engine.Timer.Wait(CoyoteTime, () => canCoyoteJump = false);
            }

            if (IsOnFloor)
                canCoyoteJump = false;

            wasOnFloor = IsOnFloor;
        }

        #endregion

        #region Wall Interaction

        public void HandleWallSlide()
        {
            if (PlayerAxis.X != 0 && IsOnWall && Velocity.Y > 0)
                wallSlideTriggered = true;

            if (!wallSlideTriggered)
                return;

            if (!IsOnWall || IsOnFloor)
                wallSlideTriggered = false;

            Velocity.Y = MathF.Min(
                Velocity.Y + WallSlideGravity,
                WallSlideGravity
            );

            if (Engine.Input.IsActionJustPressed("Jump"))
                WallJump();
        }

        public void WallJump()
        {
            AllowControl = false;
            Engine.Timer.Wait(0.06f, () => AllowControl = true);

            if (PlayerDirection == 1)
                Velocity.X = -WallJumpHorizontalSpeed;
            else if (PlayerDirection == -1)
                Velocity.X = WallJumpHorizontalSpeed;

            Velocity.Y = -WallJumpVerticalSpeed;
        }

        #endregion

        #region Visuals

        private void AnimateSprite()
        {

            if (IsOnFloor)
            {
                if (PlayerAxis.X != 0)
                    FeetSprite.PlayAnimation("Run");
                else
                    FeetSprite.PlayAnimation("Idle");
            }
            else 
            {
                FeetSprite.PlayAnimation("Fall");
            }

            if (!isAttacking)
            {
                if (IsOnFloor)
                {
                    if (PlayerAxis.X != 0)
                        Sprite.PlayAnimation("Run");
                    else
                        Sprite.PlayAnimation("Idle");
                }
                else
                {
                    Sprite.PlayAnimation("Fall");
                }
            }

            else
            {
                Sprite.PlayAnimation("Attack");
            }

            
        }

        private void FlipSprite()
        {
            if (PlayerAxis.X > 0)
            {
                Sprite.LocalSpriteEffects = SpriteEffects.None;
                FeetSprite.LocalSpriteEffects = SpriteEffects.None;
                AttackArea.LocalPosition = new Vector2(50, 5);
            }
            else if (PlayerAxis.X < 0)
            {
                FeetSprite.LocalSpriteEffects = SpriteEffects.FlipHorizontally;
                Sprite.LocalSpriteEffects = SpriteEffects.FlipHorizontally;
                AttackArea.LocalPosition = new Vector2(-35, 5);
            }
        }

        #endregion

        #region Attack

        public void HandleAttack()
        {
            if (Engine.Input.IsActionJustPressed("Attack"))
            {
                if (!isAttacking)
                {
                    Attack();
                }

                else
                {
                    BufferAttack();
                }
            }

            if (isAttacking && Sprite.IsFinished)
            {
                AttackArea.CollisionShape.Disabled = true;
                isAttacking = false;
                
                if (attackBuffer)
                {
                    attackBuffer = false;
                    Attack();
                }
            }
        }

        public void Attack()
        {
            AttackArea.CollisionShape.Disabled = false;
            attackCounter ++;
            isAttacking = true;
        }

        public void BufferAttack()
        {
            if (attackBuffer)
                return;
            
            attackBuffer = true;

            Engine.Timer.Wait(AttackBufferTime, () =>
            {
                attackBuffer = false;
            });
        }

        #endregion
    }
}
