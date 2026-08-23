
namespace Slumber;

public class FloorAttackState : State
{
  Point Axis; 

  Player p => Core.Token.Get<Player>();

  public override void OnEnter()
  {
    p.Velocity.X = 0;
    p.Sprite.PlayAnimation("Attack");
    Attack();
  }

  public override void Update(float delta)
  {
    if (Core.Input.IsActionJustPressed("Attack"))
      BufferAttack();

    if (p.Sprite.IsFinished)
    {
      p.AttackArea.Get<CollisionShape2D>().Disabled = true;
      p.Properties.IsAttacking = false;

      if (p.Properties.AttackBuffer)
      {
        p.Properties.AttackBuffer = false;
        Attack();
      }
      else
        ScreenEffects?.Invoke("IdleState");
    }
  }

  public void Attack()
  {
    p.AttackArea.Get<CollisionShape2D>().Disabled = false;
    p.Properties.AttackCounter++;
    p.Properties.IsAttacking = true;
  }

  public void BufferAttack()
  {
    if (p.Properties.AttackBuffer)
      return;

    p.Properties.AttackBuffer = true;

    Await.Span(p.Properties.AttackBufferTime, () =>
    {
      p.Properties.AttackBuffer = false;
    });
  }

  public override void Physics(float delta)
  {
    p.HandleMovementInput();
    p.HandleDeceleration(delta);
    p.ApplyGravity(delta);
    p.HandleCoyoteTime();
  }
}
