using System.Linq;

namespace Slumber;

public class SnowEmitter : Node2D
{

  ParticleEmitter2D Emitter1;
  ParticleEmitter2D Emitter2;

  EmitterParams Properties;

  public override void EnterTree()
  {
    base.EnterTree();

    Properties = EmitterParams.Identity with
    {
      Params = ParticleParams.Identity with
      {
        ColorStart = Color.White,
        ColorEnd = Color.White,
        SizeStart = 1f,
        SizeEnd = 1f,
        Lifespan = 6f,
        Speed = 20f,
        Angle = MathHelper.ToRadians(45f)
      },
      Angle = MathHelper.ToRadians(45f),
      AngleVariance = MathHelper.ToRadians(80f),
      LifespanMin = 8f,
      LifespanMax = 32f,
      SpeedMin = 10f,
      SpeedMax = 30f,
      Interval = 0.02f,
      EmitCount = 0
    };


    Emitter1 = Core.Token.Create<ParticleEmitter2D>().Set(n =>
    {
      n.Params = Properties;
    });

    Emitter2 = Core.Token.Create<ParticleEmitter2D>().Set(n =>
    {
      n.Params = Properties;
    });
  }

  public override void PhysicsUpdate(float delta)
  {
    base.PhysicsUpdate(delta);
  }

  public override void Process(float delta)
  {
    base.Process(delta);

    var c = Core.Token.Get<Camera2D>();

    Emitter1.Emit(new Vector2(MathE.Random.Next(c.Bounds.Left - 320, c.Bounds.Right - 320), c.Bounds.Top - 20), 3);
    Emitter2.Emit(new Vector2(MathE.Random.Next(c.Bounds.Right - 320, c.Bounds.Right + 320), c.Bounds.Top - 20), 3);
  }

  public override void Submit(Canvas2D canvas)
  {
    base.Submit(canvas);
  }

  public override void ExitTree()
  {
    base.ExitTree();
  }
}
