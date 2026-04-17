using System.Linq;

namespace Slumber;

public class SnowEmitter : Node2D
{

    ParticleEmitter2D Emitter1;
    ParticleEmitter2D Emitter2;

    EmitterParams Properties;

    public override void OnEnter()
    {
        base.OnEnter();

        Properties = EmitterParams.Identity with
        {
            Params = ParticleParams.Identity with
            {
                ColorStart = Color.White,
                ColorEnd = Color.White,
                SizeStart = 3f,
                SizeEnd = 3f,
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
        

        Emitter1 = Engine.Tree.Create<ParticleEmitter2D>().Set(n =>
        {
            n.Params = Properties;
        });

        Emitter2 = Engine.Tree.Create<ParticleEmitter2D>().Set(n =>
        {
            n.Params = Properties;
        });
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
    }

    public override void ProcessUpdate(float delta)
    {
        base.ProcessUpdate(delta);

        var c = Engine.Tree.Get<Camera2D>();

       Emitter1.Emit(new Vector2(MathE.Random.Next(c.Bounds.Left - 320, c.Bounds.Right - 320), c.Bounds.Top - 20), 3);
       Emitter2.Emit(new Vector2(MathE.Random.Next(c.Bounds.Right - 320, c.Bounds.Right + 320), c.Bounds.Top - 20), 3);
    }

    public override void SubmitCall()
    {
        base.SubmitCall();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
