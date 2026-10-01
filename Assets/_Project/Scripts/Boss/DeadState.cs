namespace AntLion.Boss
{
    // Final state. Leaving the previous state already stopped any attack; this stops the circling too.
    // Nothing transitions out of it.
    public class DeadState : BossState
    {
        public DeadState(BossBrain brain) : base(brain) { }

        public override string Name => "Dead";

        public override void Enter()
        {
            brain.Movement.Stop();
        }
    }
}
