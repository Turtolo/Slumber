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
      Transition?.Invoke("IdleState");
      return;
    }

    enterVel = p.Velocity;

    p.Velocity = Vector2.Zero;

    p.Properties.ThresholdReached = false;

    Core.Token.Get<PixelCamera>()?.toggleShake = true;
  }

  public override void Update(float delta)
  {
    base.Update(delta);

    if (p.Sprite.IsFinished)
    {
      Core.Token.Get<PixelCamera>()?.toggleShake = false;
      Transition?.Invoke("IdleState");
    }
  }

  public override void Physics(float delta)
  {
    base.PhysicsUpdate(delta);
  }
}
