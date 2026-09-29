namespace AntLion.Boss
{
    // Fire the next pattern, then recover. The boss keeps circling, at its faster attack speed, while it fires.
    public class AttackState : BossState
    {
        public AttackState(BossBrain brain) : base(brain) { }

        public override string Name => "Attack";

        public override void Enter()
        {
            brain.Attack.Run(brain.NextPattern());
        }

        public override void Tick()
        {
            if (!brain.Attack.IsAttacking) brain.ChangeState(brain.Recovering);
        }

        // Cleans up if something interrupts the pattern (e.g. death), so no more bullets are fired.
        public override void Exit()
        {
            brain.Attack.Stop();
        }
    }
}
