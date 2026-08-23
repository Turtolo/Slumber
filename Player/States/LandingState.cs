namespace Slumber;

public class LandingState : State
{
  Player p => Core.Token.Get<Player>();

  Vector2 enterVel;

  Point Axis;

  public override void OnEnter()
  {
    base.OnEnter();
    if (p.Properties.ThresholdReached)
    {
      p.Sprite.PlayAnimation("Landing");
    }
    else
    {
      ScreenEffects?.Invoke("IdleState");
      return;
    }

    enterVel = p.Velocity;

    p.Velocity = Vector2.Zero;

    p.Properties.ThresholdReached = false;

    Core.Token.Get<PixelCamera>()?.Shake(() => p.Sprite.IsFinished, 10, 9f);
  }

  public override void Update(float delta)
  {
    base.Update(delta);

    if (p.Sprite.IsFinished)
    {
      ScreenEffects?.Invoke("IdleState");
    }
  }

  public override void Physics(float delta)
  {
    base.PhysicsUpdate(delta);

    p.HandleCoyoteTime();
  }
}
