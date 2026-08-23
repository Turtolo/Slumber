namespace Slumber;

public class WallSlideState : State
{
  Player p => Core.Token.Get<Player>();

  public override void OnEnter()
  {
    base.OnEnter();
  }

  public override void Update(float delta)
  {
    base.Update(delta);

    if (!p.IsOnWall || p.IsOnFloor)
      ScreenEffects?.Invoke("FallState");
  }

  public override void Physics(float delta)
  {
    base.Physics(delta);
    
    p.HandleMovementInput();

    p.Velocity.Y = MathF.Min(
      p.Velocity.Y + p.Properties.WallSlideGravity,
      p.Properties.WallSlideGravity
    );

    if (Core.Input.IsActionJustPressed("Jump"))
      p.WallJump();
  }
}
