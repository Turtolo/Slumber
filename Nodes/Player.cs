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

        public int PlayerAxis;
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

        #endregion

        #region Constructors

        public Player() {}

        public override void Load()
        {
            base.Load();

            var c = Engine.Node.Create<CollisionShape2D>();
            c.Shape = new RectangleShape2D(10, 25);

            c.SetParent(this);


            var animations = AsepriteLoader.LoadAnimations(
                new("Assets/Animations/PlayerModel3Atlas"),
                PathHelper.Combine("Raw/Raw/PlayerModel3.json")
            );

            Sprite = Engine.Node.Create<AnimatedSprite2D>();

            Sprite.SetParent(this);

            Sprite.Atlas = animations;
            Sprite.Position = new Vector2(6, 9);
            Sprite.IsLooping = true;

            Depth = 5;
        }

        #endregion

        #region Update

        public override void PhysicsUpdate(float delta)
        {
            PlayerAxis = Engine.Input.GetAxis("MoveLeft", "MoveRight");
            PlayerDirection = PlayerAxis != 0 ? PlayerAxis : PlayerDirection;

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

            float targetSpeed = MoveSpeed * PlayerAxis;

            if (targetSpeed != 0)
                Velocity.X = MoveToward(Velocity.X, targetSpeed, Acceleration);
        }

        public void HandleDeceleration(float delta)
        {
            Velocity.X = PlayerAxis == 0 ? MoveToward(Velocity.X, 0, Deceleration * delta) : Velocity.X;
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
            if (PlayerAxis != 0 && IsOnWall && Velocity.Y > 0)
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
                if (!isAttacking)
                {
                    if (PlayerAxis != 0)
                        Sprite.PlayAnimation("Run");
                    else
                        Sprite.PlayAnimation("Idle");
                }
                else
                {
                    if (PlayerAxis != 0)
                        Sprite.PlayAnimation("RunAttack");
                    else
                        Sprite.PlayAnimation("Attack" + attackCounter);
                }
            }
            else
            {
                Sprite.PlayAnimation("Fall");
            }
        }

        private void FlipSprite()
        {
            if (PlayerAxis > 0)
                Sprite.SpriteEffects = SpriteEffects.None;
            else if (PlayerAxis < 0)
                Sprite.SpriteEffects = SpriteEffects.FlipHorizontally;
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
