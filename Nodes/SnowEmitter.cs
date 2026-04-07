using System.Linq;

namespace Slumber;

public class SnowEmitter : ParticleEmitter2D
{
    public override void OnEnter()
    {
        base.OnEnter();

        Properties = EmitterProperties.Identity with
        {
            ParticleProperties = ParticleProperties.Identity with
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
    }

    public override void PhysicsUpdate(float delta)
    {
        base.PhysicsUpdate(delta);
    }

    public override void ProcessUpdate(float delta)
    {
        base.ProcessUpdate(delta);

        //var toBeRemoved = Particles.Where(p => p.Info.Position.Y )
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