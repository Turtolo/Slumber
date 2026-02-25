namespace Slumber
{
    public class Test : Instance
    {
        public Test() {}

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public override void PhysicsUpdate(float dt)
        {
            base.PhysicsUpdate(dt);
        }

        public override void ProcessUpdate(float dt)
        {
            base.ProcessUpdate(dt);

            Console.WriteLine("yes");
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
}